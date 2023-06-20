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

        ShaderRegion canvasEmitSR = rakel.ApplicationReservoir.GetFullShaderRegion();

        ShaderRegion rakelEmitSR = new ShaderRegion(
            canvas.MapToPixelInRange(rakel.Info.UpperLeft),
            canvas.MapToPixelInRange(rakel.Info.UpperRight),
            canvas.MapToPixelInRange(rakel.Info.LowerLeft),
            canvas.MapToPixelInRange(rakel.Info.LowerRight)
        );

        ComputeBuffer rakelMappedInfo = rakel.TransformToRakelOrigin(
            rakelEmitSR,
            canvas,
            DebugShader);

        rakel.CalculateReservoirPixel(
            rakelMappedInfo,
            rakelEmitSR,
            DebugShader);


        // 1. Calculate rakel position based on paint height on canvas
        rakel.RecalculatePositionZ(
            canvas,
            rakelMappedInfo,
            rakelEmitSR,
            transferConfiguration.LayerThickness_MAX,
            DebugShader);

        rakel.CalculateDistanceFromRakel(
            rakelMappedInfo,
            rakelEmitSR,
            DebugShader);


        // 2. Do paint transfer and rendering
        canvas.Reservoir.Duplicate(DebugShader);

        ComputeBuffer canvasEmittedPaint = canvas.EmitPaint(
            rakel,
            canvasEmitSR,
            transferConfiguration.PickupDistance_MAX,
            transferConfiguration.PickupVolume_MIN,
            transferConfiguration.PickupVolume_MAX,
            DebugShader); ;

        rakel.ApplicationReservoir.Duplicate(DebugShader);

        rakel.PickupReservoir.Duplicate(DebugShader);

        ComputeBuffer rakelEmittedPaint = rakel.EmitPaint(
            rakelEmitSR,
            canvas,
            rakelMappedInfo,
            transferConfiguration.EmitDistance_MAX,
            transferConfiguration.EmitVolume_MIN,
            transferConfiguration.EmitVolume_MAX,
            transferConfiguration.EmitVolumeApplicationReservoirRate,
            transferConfiguration.EmitVolumePickupReservoirRate,
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
