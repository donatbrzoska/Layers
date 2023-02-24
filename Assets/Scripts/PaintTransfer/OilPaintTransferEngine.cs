using System;
using UnityEngine;

public class OilPaintTransferEngine
{
    private ShaderRegionFactory ShaderRegionFactory;
    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

    public OilPaintTransferEngine(ShaderRegionFactory shaderRegionFactory)
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

        ShaderRegion emitSR = ShaderRegionFactory.Create(
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

        rakel.ApplicationReservoir.Duplicate(
            transferConfiguration.ReservoirDiscardVolumeThreshold,
            transferConfiguration.ReservoirSmoothingKernelSize);

        rakel.PickupReservoir.Duplicate(
            transferConfiguration.ReservoirDiscardVolumeThreshold,
            transferConfiguration.ReservoirSmoothingKernelSize);

        ComputeBuffer rakelEmittedPaint = rakel.EmitPaint(
            emitSR,
            wsc,
            transferConfiguration.MapMode);

        oilPaintCanvas.ApplyPaint(
            emitSR,
            rakelEmittedPaint);

        oilPaintCanvas.UpdateColorTexture(emitSR);
        oilPaintCanvas.UpdateNormalMap(normalsSR);
    }
}
