using System;
using UnityEngine;

public class TransferEngine
{
    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

    public TransferEngine() { }

    // Position is located at Rakel Anchor
    // Rotation 0 means Rakel is directed to the right
    // Tilt 0 means Rakel is parallel to canvas
    public void SimulateStep(
        Vector3 rakelPosition, int autoZEnabled, float rakelPressure, float rakelRotation, float rakelTilt,
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

        rakel.UpdateState(rakelPosition, autoZEnabled, rakelPressure, rakelRotation, rakelTilt);

        ShaderRegion canvasEmitSR = rakel.Reservoir.GetFullShaderRegion();

        ShaderRegion rakelEmitSR = new ShaderRegion(
            canvas.MapToPixelInRange(rakel.Info.UpperLeft),
            canvas.MapToPixelInRange(rakel.Info.UpperRight),
            canvas.MapToPixelInRange(rakel.Info.LowerLeft),
            canvas.MapToPixelInRange(rakel.Info.LowerRight)
        );

        ComputeBuffer rakelMappedInfo = rakel.CalculateRakelMappedInfo(
            rakelEmitSR,
            canvas);


        // 1. Calculate rakel position based on paint height on canvas
        rakel.RecalculatePositionBaseZ(
            canvas,
            rakelMappedInfo,
            rakelEmitSR,
            transferConfiguration.LayerThickness_MAX);

        // Now that the rakel position is calculated, we can actually
        // determine the distance to the rakel and the volume to emit also
        rakel.Reservoir.Duplicate(false);
        canvas.Reservoir.Duplicate(false);

        rakel.CalculateRakelMappedInfo_Part2(
            canvas,
            rakelMappedInfo,
            rakelEmitSR,
            transferConfiguration.EmitVolume_MIN);


        // 2. Do paint transfer and rendering
        PaintGrid canvasEmittedPaint = canvas.EmitPaint(
            rakel,
            canvasEmitSR,
            //transferConfiguration.PickupDistance_MAX,
            transferConfiguration.PickupVolume_MIN
            //transferConfiguration.PickupVolume_MAX,
            );

        PaintGrid rakelEmittedPaint = rakel.EmitPaint(
            rakelEmitSR,
            canvas,
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
            ));
    }
}
