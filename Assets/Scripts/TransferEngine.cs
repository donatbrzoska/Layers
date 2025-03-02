using System.Collections.Generic;
using UnityEngine;

public class TransferEngine
{
    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

    private bool DelayedExection;
    private TransferConfiguration TransferConfig;
    private InputStateSource InputStateSource;

    private Rakel Rakel;
    private ComputeBuffer CanvasMappedInfo;

    private Canvas_ Canvas;
    private ComputeBuffer RakelMappedInfo;

    public TransferEngine(bool delayedExecution, TransferConfiguration transferConfig, InputStateSource inputStateSource)
    {
        DelayedExection = delayedExecution;
        TransferConfig = transferConfig;
        InputStateSource = inputStateSource;
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

    public void ProcessSteps(int n=0)
    {
        while (InputStateSource.HasNext())
        {
            SimulateStep(InputStateSource.Next());

            // only used for delayed execution
            if (--n == 0)
            {
                break;
            }
        }
    }

    public bool IsDone()
    {
        if (DelayedExection && InputStateSource.HasNext())
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
    public void SimulateStep(InputState inputState)
    {
        // prevent double application on the same pixel
        Vector3 rakelPosition = Canvas.AlignToPixelGrid(inputState.Position);
        if (Canvas.MapToPixel(rakelPosition).Equals(PreviousApplyPosition))
        {
            return;
        }
        else
        {
            PreviousApplyPosition = Canvas.MapToPixel(rakelPosition);
        }

        //Debug.Log("Applying at x=" + wsc.MapToPixel(rakelPosition));

        bool finalUpdateForStroke = !inputState.PositionAutoBaseZEnabled; // (when auto Z is disabled, RecalculatePositionBaseZ won't do anything)
        Rakel.UpdateState(
            rakelPosition,
            TransferConfig.BaseSink_MAX, TransferConfig.LayerSink_MAX_Ratio, TransferConfig.TiltSink_MAX,
            inputState.PositionAutoBaseZEnabled, false, finalUpdateForStroke,
            inputState.Pressure, inputState.Rotation, inputState.Tilt);

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
            TransferConfig.CanvasVolumeReduceFunction,
            TransferConfig.RakelVolumeReduceFunction,
            TransferConfig.ReadjustZToRakelVolume,
            TransferConfig.ReadjustZToCanvasVolume,
            TransferConfig.LayerThickness_MAX,
            TransferConfig.TiltAdjustLayerThickness,
            TransferConfig.BaseSink_MAX,
            TransferConfig.LayerSink_MAX_Ratio,
            TransferConfig.TiltSink_MAX);

        // Now that the rakel position is calculated, we can actually
        // determine the distance to the rakel and the volume to emit also
        Rakel.CalculateRakelMappedInfo_Part2(
            Canvas,
            RakelMappedInfo,
            rakelEmitSR,
            TransferConfig.EmitDistance_MAX,
            TransferConfig.EmitVolume_MIN);

        // 3.Do paint transfer and rendering
        if (TransferConfig.CanvasSnapshotBufferEnabled)
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
            TransferConfig.PickupDistance_MAX,
            TransferConfig.PickupVolume_MIN,
            rakelEmitSR,
            TransferConfig.CanvasSnapshotBufferEnabled,
            TransferConfig.DeletePickedUpFromCSB,
            TransferConfig.PaintDoesPickup);

        Rakel.EmitPaint(
            Canvas,
            RakelMappedInfo,
            rakelEmitSR);

        Canvas.ApplyInputBuffer(rakelEmitSR, TransferConfig.CanvasDiffuseDepth, TransferConfig.CanvasDiffuseRatio);

        Rakel.ApplyInputBuffer(canvasEmitSR, TransferConfig.RakelDiffuseDepth, TransferConfig.RakelDiffuseRatio);

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
