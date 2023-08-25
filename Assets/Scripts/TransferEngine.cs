using System.Collections.Generic;
using UnityEngine;

public struct SimulationStep
{
    public Vector3 RakelPosition;
    public bool AutoZEnabled;
    public float RakelPressure;
    public float RakelRotation;
    public float RakelTilt;
    public TransferConfiguration TransferConfig;

    public SimulationStep(
        Vector3 rakelPosition, bool autoZEnabled, float rakelPressure, float rakelRotation, float rakelTilt,
        TransferConfiguration transferConfig)
    {
        RakelPosition = rakelPosition;
        AutoZEnabled = autoZEnabled;
        RakelPressure = rakelPressure;
        RakelRotation = rakelRotation;
        RakelTilt = rakelTilt;
        TransferConfig = transferConfig;
    }
}

public class TransferEngine
{
    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

    private bool DelayedExection;
    private Queue<SimulationStep> SimulationSteps;

    private Rakel Rakel;
    private ComputeBuffer CanvasMappedInfo;

    private Canvas_ Canvas;
    private ComputeBuffer RakelMappedInfo;

    public TransferEngine(bool delayedExecution)
    {
        DelayedExection = delayedExecution;
        if (delayedExecution)
        {
            SimulationSteps = new Queue<SimulationStep>();
        }
    }

    public void SetRakel(Rakel rakel)
    {
        if (CanvasMappedInfo == null || Rakel.Reservoir.Size2D != rakel.Reservoir.Size2D)
        {
            CanvasMappedInfo?.Dispose();
            CanvasMappedInfo = MappedInfo.CreateBuffer(rakel.Reservoir.Size2D);
        }
        Rakel = rakel;
    }

    public void SetCanvas(Canvas_ canvas)
    {
        if (RakelMappedInfo == null || Canvas.Reservoir.Size2D != canvas.Reservoir.Size2D)
        {
            RakelMappedInfo?.Dispose();
            RakelMappedInfo = MappedInfo.CreateBuffer(canvas.Reservoir.Size2D);
        }
        Canvas = canvas;
    }

    public void EnqueueOrRun(
        Vector3 rakelPosition, bool autoZEnabled, float rakelPressure, float rakelRotation, float rakelTilt,
        TransferConfiguration transferConfig)
    {
        SimulationStep s = new SimulationStep(rakelPosition, autoZEnabled, rakelPressure, rakelRotation, rakelTilt, transferConfig);

        if (DelayedExection)
        {
            SimulationSteps.Enqueue(s);
        }
        else
        {
            SimulateStep(s.RakelPosition, s.AutoZEnabled, s.RakelPressure, s.RakelRotation, s.RakelTilt, s.TransferConfig);
        }
    }

    public void ProcessSteps(int n)
    {
        if (DelayedExection)
        {
            while (n-- >= 0 && SimulationSteps.Count > 0)
            {
                SimulationStep s = SimulationSteps.Dequeue();
                SimulateStep(s.RakelPosition, s.AutoZEnabled, s.RakelPressure, s.RakelRotation, s.RakelTilt, s.TransferConfig);
            }
        }
    }

    public bool IsDone()
    {
        if (DelayedExection && SimulationSteps.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void NewStroke(
        bool tiltNoiseEnabled, float tiltNoiseFrequency, float tiltNoiseAmplitude, float floatingZLength,
        bool csbEnabled)
    {
        Rakel.NewStroke(tiltNoiseEnabled, tiltNoiseFrequency, tiltNoiseAmplitude, floatingZLength);

        if (csbEnabled)
        {
            Canvas.Reservoir.DoSnapshot(false);
        }
    }

    // Position is located at Rakel Anchor
    // Rotation 0 means Rakel is directed to the right
    // Tilt 0 means Rakel is parallel to canvas
    public void SimulateStep(
        Vector3 rakelPosition, bool autoZEnabled, float rakelPressure, float rakelRotation, float rakelTilt,
        TransferConfiguration transferConfig)
    {
        // prevent double application on the same pixel
        rakelPosition = Canvas.AlignToPixelGrid(rakelPosition);
        if (Canvas.MapToPixel(rakelPosition).Equals(PreviousApplyPosition))
        {
            return;
        }
        else
        {
            PreviousApplyPosition = Canvas.MapToPixel(rakelPosition);
        }

        //Debug.Log("Applying at x=" + wsc.MapToPixel(rakelPosition));

        bool finalUpdateForStroke = !autoZEnabled; // (when auto Z is disabled, RecalculatePositionBaseZ won't do anything)
        Rakel.UpdateState(
            rakelPosition,
            transferConfig.BaseSink_MAX, transferConfig.LayerSink_MAX_Ratio, transferConfig.TiltSink_MAX,
            autoZEnabled, false, finalUpdateForStroke,
            rakelPressure, rakelRotation, rakelTilt);

        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();

        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight)
        );


        // 1. duplicate, so we can sample from there and only delete from original
        Rakel.Reservoir.DoImprintCopy(canvasEmitSR, false);
        Canvas.Reservoir.DoImprintCopy(rakelEmitSR, false);


        // 2. Calculate rakel position based on paint height on canvas
        //    For this, we already calculate parts of rakel mapped info
        Rakel.CalculateRakelMappedInfo(
            rakelEmitSR,
            Canvas,
            RakelMappedInfo);

        Rakel.RecalculatePositionBaseZ(
            Canvas,
            RakelMappedInfo,
            rakelEmitSR,
            transferConfig.CanvasVolumeReduceFunction,
            transferConfig.RakelVolumeReduceFunction,
            transferConfig.ReadjustZToRakelVolume,
            transferConfig.ReadjustZToCanvasVolume,
            transferConfig.LayerThickness_MAX,
            transferConfig.TiltAdjustLayerThickness,
            transferConfig.BaseSink_MAX,
            transferConfig.LayerSink_MAX_Ratio,
            transferConfig.TiltSink_MAX);

        // Now that the rakel position is calculated, we can actually
        // determine the distance to the rakel and the volume to emit also
        Rakel.CalculateRakelMappedInfo_Part2(
            Canvas,
            RakelMappedInfo,
            rakelEmitSR,
            transferConfig.EmitDistance_MAX,
            transferConfig.EmitVolume_MIN);

        // 3.Do paint transfer and rendering
        if (transferConfig.CanvasSnapshotBufferEnabled)
        {
            //Keep canvas snapshot buffer(CSB) up to date:
            // -> Copy any paint into CSB, that might get picked up in the next
            //    simulation step(not this one)
            // -> Padding of size of rakel reservoir is overkill in the most cases
            //    but also delivers update guarantee, no matter how the rakel is
            //    shaped and where the rakel anchor is located.
            ShaderRegion updateSR = new ShaderRegion(
                Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
                Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
                Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
                Canvas.MapToPixelInRange(Rakel.Info.LowerRight),
                Mathf.Max(Rakel.Reservoir.Size.x, Rakel.Reservoir.Size.y)
            );
            Canvas.Reservoir.DoSnapshotUpdate(
                RakelMappedInfo,
                Canvas.Reservoir.Size2D,
                Rakel.Reservoir.Size,
                updateSR);
        }

        Canvas.EmitPaint(
            Rakel,
            CanvasMappedInfo,
            canvasEmitSR,
            transferConfig.PickupDistance_MAX,
            transferConfig.PickupVolume_MIN,
            rakelEmitSR,
            transferConfig.CanvasSnapshotBufferEnabled,
            transferConfig.DeletePickedUpFromCSB,
            transferConfig.PaintDoesPickup);

        Rakel.EmitPaint(
            Canvas,
            RakelMappedInfo,
            rakelEmitSR);

        Canvas.ApplyInputBuffer(rakelEmitSR, transferConfig.CanvasDiffuseDepth, transferConfig.CanvasDiffuseRatio);

        Rakel.ApplyInputBuffer(canvasEmitSR, transferConfig.RakelDiffuseDepth, transferConfig.RakelDiffuseRatio);

        Canvas.Render(
            new ShaderRegion(
                Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
                Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
                Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
                Canvas.MapToPixelInRange(Rakel.Info.LowerRight),
                1 // Padding because normal calculation is also based on pixels around
            ),
            false);
    }

    public void Dispose()
    {
        RakelMappedInfo.Dispose();
        CanvasMappedInfo.Dispose();
    }
}
