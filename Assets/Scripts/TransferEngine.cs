using System;
using UnityEngine;

public class TransferEngine
{
    private bool DebugShader;
    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

    public TransferEngine(bool debugShader)
    {
        DebugShader = debugShader;
    }

    // Position is located at Rakel Anchor
    // Rotation 0 means Rakel is directed to the right
    // Tilt 0 means Rakel is parallel to canvas
    public void SimulateStep(
        Vector3 rakelPosition, float rakelPressure, float rakelRotation, float rakelTilt,
        TransferConfiguration transferConfiguration,
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

        rakel.UpdateState(rakelPosition, rakelPressure, rakelRotation, rakelTilt, DebugShader);

        ShaderRegion canvasEmitSR = rakel.Reservoir.GetFullShaderRegion();

        ShaderRegion rakelEmitSR = new ShaderRegion(
            canvas.MapToPixelInRange(rakel.Info.UpperLeft),
            canvas.MapToPixelInRange(rakel.Info.UpperRight),
            canvas.MapToPixelInRange(rakel.Info.LowerLeft),
            canvas.MapToPixelInRange(rakel.Info.LowerRight)
        );

        ComputeBuffer rakelMappedInfo = rakel.CalculateRakelMappedInfo(
            rakelEmitSR,
            canvas,
            DebugShader);


        // 1. Calculate rakel position based on paint height on canvas
        rakel.RecalculatePositionBaseZ(
            canvas,
            rakelMappedInfo,
            rakelEmitSR,
            transferConfiguration.LayerThickness_MAX,
            DebugShader);

        // Now that the rakel position is calculated, we can actually
        // determine the distance to the rakel and the volume to emit also
        rakel.Reservoir.Duplicate(DebugShader);
        canvas.Reservoir.Duplicate(DebugShader);

        rakel.CalculateRakelMappedInfo_Part2(
            canvas,
            rakelMappedInfo,
            rakelEmitSR,
            transferConfiguration.EmitVolume_MIN,
            DebugShader);


        // 2. Do paint transfer and rendering
        PaintGrid canvasEmittedPaint = canvas.EmitPaint(
            rakel,
            canvasEmitSR,
            //transferConfiguration.PickupDistance_MAX,
            transferConfiguration.PickupVolume_MIN,
            //transferConfiguration.PickupVolume_MAX,
            DebugShader);

        PaintGrid rakelEmittedPaint = rakel.EmitPaint(
            rakelEmitSR,
            canvas,
            rakelMappedInfo,
            DebugShader);

        canvas.ApplyPaint(
            rakelEmitSR,
            rakelEmittedPaint,
            DebugShader);

        rakel.ApplyPaint(
            canvasEmitSR,
            canvasEmittedPaint,
            DebugShader);

        canvas.Render(
            new ShaderRegion(
                canvas.MapToPixelInRange(rakel.Info.UpperLeft),
                canvas.MapToPixelInRange(rakel.Info.UpperRight),
                canvas.MapToPixelInRange(rakel.Info.LowerLeft),
                canvas.MapToPixelInRange(rakel.Info.LowerRight),
                1), // Padding because normal calculation is also based on pixels around
            DebugShader);
    }
}
