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
    public Rakel Rakel;
    public Canvas_ Canvas;

    public SimulationStep(
        Vector3 rakelPosition, bool autoZEnabled, float rakelPressure, float rakelRotation, float rakelTilt,
        TransferConfiguration transferConfig, Rakel rakel, Canvas_ canvas)
    {
        RakelPosition = rakelPosition;
        AutoZEnabled = autoZEnabled;
        RakelPressure = rakelPressure;
        RakelRotation = rakelRotation;
        RakelTilt = rakelTilt;
        TransferConfig = transferConfig;
        Rakel = rakel;
        Canvas = canvas;
    }
}

public class TransferEngine
{
    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

    private bool DelayedExection;
    private Queue<SimulationStep> SimulationSteps;

    public TransferEngine(bool delayedExecution)
    {
        DelayedExection = delayedExecution;
        if (delayedExecution)
        {
            SimulationSteps = new Queue<SimulationStep>();
        }
    }

    public void EnqueueOrRun(
        Vector3 rakelPosition, bool autoZEnabled, float rakelPressure, float rakelRotation, float rakelTilt,
        TransferConfiguration transferConfig,
        Rakel rakel,
        Canvas_ canvas)
    {
        SimulationStep s = new SimulationStep(rakelPosition, autoZEnabled, rakelPressure, rakelRotation, rakelTilt, transferConfig, rakel, canvas);

        if (DelayedExection)
        {
            SimulationSteps.Enqueue(s);
        }
        else
        {
            SimulateStep(s.RakelPosition, s.AutoZEnabled, s.RakelPressure, s.RakelRotation, s.RakelTilt, s.TransferConfig, s.Rakel, s.Canvas);
        }
    }

    public void ProcessSteps(int n)
    {
        if (DelayedExection)
        {
            while (n-- >= 0 && SimulationSteps.Count > 0)
            {
                SimulationStep s = SimulationSteps.Dequeue();
                SimulateStep(s.RakelPosition, s.AutoZEnabled, s.RakelPressure, s.RakelRotation, s.RakelTilt, s.TransferConfig, s.Rakel, s.Canvas);
            }
        }
    }

    public bool Done()
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
        Rakel rakel, bool tiltNoiseEnabled, float tiltNoiseFrequency, float tiltNoiseAmplitude, float floatingZLength,
        Canvas_ canvas, bool csbEnabled)
    {
        rakel.NewStroke(tiltNoiseEnabled, tiltNoiseFrequency, tiltNoiseAmplitude, floatingZLength);

        if (csbEnabled)
        {
            canvas.Reservoir.DoSnapshot(false);
        }
    }

    // Position is located at Rakel Anchor
    // Rotation 0 means Rakel is directed to the right
    // Tilt 0 means Rakel is parallel to canvas
    public void SimulateStep(
        Vector3 rakelPosition, bool autoZEnabled, float rakelPressure, float rakelRotation, float rakelTilt,
        TransferConfiguration transferConfig,
        Rakel rakel,
        Canvas_ canvas)
    {
        // prevent double application on the same pixel
        rakelPosition = canvas.AlignToPixelGrid(rakelPosition);
        if (canvas.MapToPixel(rakelPosition).Equals(PreviousApplyPosition))
        {
            return;
        }
        else
        {
            PreviousApplyPosition = canvas.MapToPixel(rakelPosition);
        }

        //Debug.Log("Applying at x=" + wsc.MapToPixel(rakelPosition));

        bool finalUpdateForStroke = !autoZEnabled; // (when auto Z is disabled, RecalculatePositionBaseZ won't do anything)
        rakel.UpdateState(
            rakelPosition,
            transferConfig.BaseSink_MAX, transferConfig.LayerSink_MAX_Ratio, transferConfig.TiltSink_MAX,
            autoZEnabled, false, finalUpdateForStroke,
            rakelPressure, rakelRotation, rakelTilt);

        ShaderRegion canvasEmitSR = rakel.Reservoir.GetFullShaderRegion();

        ShaderRegion rakelEmitSR = new ShaderRegion(
            canvas.MapToPixelInRange(rakel.Info.UpperLeft),
            canvas.MapToPixelInRange(rakel.Info.UpperRight),
            canvas.MapToPixelInRange(rakel.Info.LowerLeft),
            canvas.MapToPixelInRange(rakel.Info.LowerRight)
        );


        // 1. duplicate, so we can sample from there and only delete from original
        rakel.Reservoir.DoImprintCopy(canvasEmitSR, false);
        canvas.Reservoir.DoImprintCopy(rakelEmitSR, false);


        // 2. Calculate rakel position based on paint height on canvas
        //    For this, we already calculate parts of rakel mapped info
        ComputeBuffer rakelMappedInfo = rakel.CalculateRakelMappedInfo(
            rakelEmitSR,
            canvas);

        rakel.RecalculatePositionBaseZ(
            canvas,
            rakelMappedInfo,
            rakelEmitSR,
            transferConfig.ReadjustZToRakelVolume,
            transferConfig.ReadjustZToCanvasVolume,
            transferConfig.LayerThickness_MAX,
            transferConfig.BaseSink_MAX,
            transferConfig.LayerSink_MAX_Ratio,
            transferConfig.TiltSink_MAX);

        // Now that the rakel position is calculated, we can actually
        // determine the distance to the rakel and the volume to emit also
        rakel.CalculateRakelMappedInfo_Part2(
            canvas,
            rakelMappedInfo,
            rakelEmitSR,
            transferConfig.EmitDistance_MAX,
            transferConfig.EmitVolume_MIN);


        // 3. Do paint transfer and rendering
        if (transferConfig.CanvasSnapshotBufferEnabled)
        {
            // Keep canvas snapshot buffer (CSB) up to date:
            // -> Copy any paint into CSB, that might get picked up in the next
            //    simulation step (not this one)
            // -> Padding of size of rakel reservoir is overkill in the most cases
            //    but also delivers update guarantee, no matter how the rakel is
            //    shaped and where the rakel anchor is located.
            ShaderRegion updateSR = new ShaderRegion(
                canvas.MapToPixelInRange(rakel.Info.UpperLeft),
                canvas.MapToPixelInRange(rakel.Info.UpperRight),
                canvas.MapToPixelInRange(rakel.Info.LowerLeft),
                canvas.MapToPixelInRange(rakel.Info.LowerRight),
                Mathf.Max(rakel.Reservoir.Size.x, rakel.Reservoir.Size.y)
            );
            canvas.Reservoir.DoSnapshotUpdate(
                rakelMappedInfo,
                rakelEmitSR,
                rakel.Reservoir.Size,
                updateSR);
        }

        PaintGrid canvasEmittedPaint = canvas.EmitPaint(
            rakel,
            canvasEmitSR,
            transferConfig.PickupDistance_MAX,
            transferConfig.PickupVolume_MIN,
            rakelEmitSR,
            transferConfig.CanvasSnapshotBufferEnabled,
            transferConfig.DeletePickedUpFromCSB,
            transferConfig.PaintDoesPickup);

        PaintGrid rakelEmittedPaint = rakel.EmitPaint(
            rakelEmitSR,
            rakelMappedInfo);

        canvas.ApplyPaint(
            rakelEmitSR,
            rakelEmittedPaint);

        rakel.ApplyPaint(
            canvasEmitSR,
            canvasEmittedPaint);

        canvas.Render(
            new ShaderRegion(
                canvas.MapToPixelInRange(rakel.Info.UpperLeft),
                canvas.MapToPixelInRange(rakel.Info.UpperRight),
                canvas.MapToPixelInRange(rakel.Info.LowerLeft),
                canvas.MapToPixelInRange(rakel.Info.LowerRight),
                1 // Padding because normal calculation is also based on pixels around
            ),
            false);
    }
}
