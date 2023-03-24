using System;
using UnityEngine;

public class TransferEngine
{
    private ShaderRegionFactory ShaderRegionFactory;
    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

    public TransferEngine(ShaderRegionFactory shaderRegionFactory)
    {
        ShaderRegionFactory = shaderRegionFactory;
    }

    // Position is located at Rakel Anchor
    // Rotation 0 means Rakel is directed to the right
    // Tilt 0 means Rakel is flat on canvas
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

        ShaderRegion canvasEmitSR = rakel.ApplicationReservoir.GetShaderRegion();

        ShaderRegion rakelEmitSR = ShaderRegionFactory.Create(
            wsc.MapToPixelInRange(rakel.UpperLeft),
            wsc.MapToPixelInRange(rakel.UpperRight),
            wsc.MapToPixelInRange(rakel.LowerLeft),
            wsc.MapToPixelInRange(rakel.LowerRight),
            1 // Padding because interpolation reaches pixels that are not directly under the rakel
        );

        ShaderRegion rerenderSR = ShaderRegionFactory.Create(
            wsc.MapToPixelInRange(rakel.UpperLeft),
            wsc.MapToPixelInRange(rakel.UpperRight),
            wsc.MapToPixelInRange(rakel.LowerLeft),
            wsc.MapToPixelInRange(rakel.LowerRight),
            2 // Padding of 2 because normals of the previously set pixels around also have to be recalculated
        );

        canvas.Reservoir.Duplicate(
            transferConfiguration.ReservoirDiscardVolumeThreshold,
            transferConfiguration.ReservoirSmoothingKernelSize, false);

        ComputeBuffer canvasEmittedPaint = canvas.EmitPaint(
            rakel,
            canvasEmitSR,
            transferConfiguration.MapMode,
            transferConfiguration.PickupVolume, false);

        rakel.ApplicationReservoir.Duplicate(
            transferConfiguration.ReservoirDiscardVolumeThreshold,
            transferConfiguration.ReservoirSmoothingKernelSize, false);

        rakel.PickupReservoir.Duplicate(
            transferConfiguration.ReservoirDiscardVolumeThreshold,
            transferConfiguration.ReservoirSmoothingKernelSize, false);

        ComputeBuffer rakelEmittedPaint = rakel.EmitPaint(
            rakelEmitSR,
            wsc,
            transferConfiguration.MapMode,
            transferConfiguration.EmitVolumeApplicationReservoir,
            transferConfiguration.EmitVolumePickupReservoir, false);

        canvas.ApplyPaint(
            rakelEmitSR,
            rakelEmittedPaint);

        rakel.ApplyPaint(
            canvasEmitSR,
            canvasEmittedPaint);

        canvas.Render(rerenderSR);
    }
}
