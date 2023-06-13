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

        ShaderRegion canvasEmitSR = rakel.ApplicationReservoir.GetFullShaderRegion();

        ShaderRegion rakelEmitSR = new ShaderRegion(
            wsc.MapToPixelInRange(rakel.UpperLeft),
            wsc.MapToPixelInRange(rakel.UpperRight),
            wsc.MapToPixelInRange(rakel.LowerLeft),
            wsc.MapToPixelInRange(rakel.LowerRight),
            1 // Padding because interpolation reaches pixels that are not directly under the rakel
        );

        ShaderRegion rerenderSR = new ShaderRegion(
            wsc.MapToPixelInRange(rakel.UpperLeft),
            wsc.MapToPixelInRange(rakel.UpperRight),
            wsc.MapToPixelInRange(rakel.LowerLeft),
            wsc.MapToPixelInRange(rakel.LowerRight),
            2 // Padding of 2 because normals of the previously set pixels around also have to be recalculated
        );

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
            wsc,
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
            rerenderSR,
            DebugShader);
    }
}
