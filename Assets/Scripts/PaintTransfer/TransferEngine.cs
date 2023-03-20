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
        OilPaintCanvas oilPaintCanvas)
    {
        WorldSpaceCanvas wsc = oilPaintCanvas.WorldSpaceCanvas;

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

        ShaderRegion normalsSR = ShaderRegionFactory.Create(
            wsc.MapToPixelInRange(rakel.UpperLeft),
            wsc.MapToPixelInRange(rakel.UpperRight),
            wsc.MapToPixelInRange(rakel.LowerLeft),
            wsc.MapToPixelInRange(rakel.LowerRight),
            2 // Padding of 2 because normals of the previously set pixels around also have to be recalculated
        );

        /*
         * When using double buffering, a reservoir that is only partially written
         * to needs to be duplicated before doing so.
         */
        oilPaintCanvas.Reservoir.Duplicate();
        rakel.ApplicationReservoir.Duplicate();
        rakel.PickupReservoir.Duplicate();

        oilPaintCanvas.EmitPaint(
            rakel,
            canvasEmitSR,
            transferConfiguration.MapMode,
            transferConfiguration.PickupVolume, false);

        /*
         * When using double buffering, a reservoir that is only partially written
         * to needs to be duplicated before doing so.
         */
        oilPaintCanvas.Reservoir.Duplicate();
        rakel.ApplicationReservoir.Duplicate();
        rakel.PickupReservoir.Duplicate();

        rakel.EmitPaint(
            rakelEmitSR,
            oilPaintCanvas,
            transferConfiguration.MapMode,
            transferConfiguration.EmitVolumeApplicationReservoir,
            transferConfiguration.EmitVolumePickupReservoir, false);

        oilPaintCanvas.UpdateColorTexture(rakelEmitSR);
        oilPaintCanvas.UpdateNormalMap(normalsSR);

        /*
         * This may be moved to the duplication shader for optimization but
         * since the smoothing probably won't be used anyways we don't do that
         * so the code is more readable
         */
        if (transferConfiguration.ReservoirSmoothingKernelSize > 1)
        {
            // probably only maybe needed for bilinear interpolation mapping
            oilPaintCanvas.Reservoir.Smooth(
                transferConfiguration.ReservoirDiscardVolumeThreshold,
                transferConfiguration.ReservoirSmoothingKernelSize, false);

            // probably only maybe needed for bilinear interpolation mapping
            rakel.ApplicationReservoir.Smooth(
            transferConfiguration.ReservoirDiscardVolumeThreshold,
            transferConfiguration.ReservoirSmoothingKernelSize, false);

            // probably only maybe needed for bilinear interpolation mapping
            rakel.PickupReservoir.Smooth(
                transferConfiguration.ReservoirDiscardVolumeThreshold,
                transferConfiguration.ReservoirSmoothingKernelSize, false);
        }
    }
}
