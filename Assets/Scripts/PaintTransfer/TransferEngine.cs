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
        Vector3 rakelPosition, float rakelRotation, float rakelTilt,
        TransferConfiguration transferConfiguration,
        Rakel rakel,
        Canvas_ canvas)
    {
        WorldSpaceCanvas wsc = canvas.WorldSpaceCanvas;

        // prevent double application on the same pixel
        rakelPosition = wsc.AlignToPixelGrid(rakelPosition);
        if (wsc.MapToPixel(rakelPosition).Equals(PreviousApplyPosition))
        {
            return;
        }
        else
        {
            PreviousApplyPosition = wsc.MapToPixel(rakelPosition);
        }

        //Debug.Log("Applying at x=" + wsc.MapToPixel(rakelPosition));

        rakel.UpdateState(rakelPosition, rakelRotation, rakelTilt);

        ShaderCalculation canvasEmitSC = rakel.ApplicationReservoir.GetShaderCalculation();

        ShaderCalculation rakelEmitSC = new ShaderCalculation(
            wsc.MapToPixelInRange(rakel.UpperLeft),
            wsc.MapToPixelInRange(rakel.UpperRight),
            wsc.MapToPixelInRange(rakel.LowerLeft),
            wsc.MapToPixelInRange(rakel.LowerRight),
            1 // Padding because interpolation reaches pixels that are not directly under the rakel
        );

        ShaderCalculation rerenderSC = new ShaderCalculation(
            wsc.MapToPixelInRange(rakel.UpperLeft),
            wsc.MapToPixelInRange(rakel.UpperRight),
            wsc.MapToPixelInRange(rakel.LowerLeft),
            wsc.MapToPixelInRange(rakel.LowerRight),
            2 // Padding of 2 because normals of the previously set pixels around also have to be recalculated
        );

        canvas.Reservoir.Duplicate(false);

        ComputeBuffer canvasEmittedPaint = canvas.EmitPaint(
            rakel,
            canvasEmitSC,
            transferConfiguration.PickupDistance_MAX,
            transferConfiguration.PickupVolume_MIN,
            transferConfiguration.PickupVolume_MAX, false); ;

        rakel.ApplicationReservoir.Duplicate(false);

        rakel.PickupReservoir.Duplicate(false);

        ComputeBuffer rakelEmittedPaint = rakel.EmitPaint(
            rakelEmitSC,
            wsc,
            transferConfiguration.EmitDistance_MAX,
            transferConfiguration.EmitVolumeApplicationReservoir_MIN,
            transferConfiguration.EmitVolumeApplicationReservoir_MAX,
            transferConfiguration.EmitVolumePickupReservoir_MIN,
            transferConfiguration.EmitVolumePickupReservoir_MAX, false);

        canvas.ApplyPaint(
            rakelEmitSC,
            rakelEmittedPaint);

        rakel.ApplyPaint(
            canvasEmitSC,
            canvasEmittedPaint);

        canvas.Render(rerenderSC);
    }
}
