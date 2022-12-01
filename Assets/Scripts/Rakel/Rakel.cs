using UnityEngine;

public class Rakel : IRakel
{
    private float Length; // world space
    private float Width; // world space
    private int Resolution; // pixels per 1 world space
    private WorldSpaceCanvas WorldSpaceCanvas;

    public Rakel(float length, float width, int resolution, WorldSpaceCanvas wsc)
    {
        Length = length;
        Width = width;
        Resolution = resolution;
        WorldSpaceCanvas = wsc;
    }

    // Position is located halfway through the rakel, at the handle
    // Rotation 0 means Rakel is directed to the right
    // Tilt 0 means Rakel is flat on canvas
    public void Apply(Vector3 rakelPosition, float rakelRotation, float rakelTilt, RenderTexture target)
    {
        Vector3 ulOrigin = new Vector3(0, Length, 0);
        Vector3 urOrigin = new Vector3(Width, Length, 0);
        Vector3 llOrigin = new Vector3(0, 0, 0);
        Vector3 lrOrigin = new Vector3(Width, 0, 0);

        Quaternion tiltQuaternion = Quaternion.AngleAxis(rakelTilt, Vector3.up);
        Vector3 urTilted = tiltQuaternion * urOrigin;
        Vector3 lrTilted = tiltQuaternion * lrOrigin;

        Quaternion rotationQuaternion = Quaternion.AngleAxis(rakelRotation, Vector3.back);
        Vector3 ulRotated = rotationQuaternion * ulOrigin;
        Vector3 urRotated = rotationQuaternion * urTilted;
        Vector3 llRotated = rotationQuaternion * llOrigin;
        Vector3 lrRotated = rotationQuaternion * lrTilted;

        Vector3 positionTranslation = rakelPosition - new Vector3(Width, Length / 2, 0);
        Vector3 ulFinal = ulRotated + positionTranslation;
        Vector3 urFinal = urRotated + positionTranslation;
        Vector3 llFinal = llRotated + positionTranslation;
        Vector3 lrFinal = lrRotated + positionTranslation;
        //UnityEngine.Debug.Log("ulFinal " + ulFinal);
        //UnityEngine.Debug.Log("urFinal " + urFinal);
        //UnityEngine.Debug.Log("llFinal " + llFinal);
        //UnityEngine.Debug.Log("lrFinal " + lrFinal);

        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(
            WorldSpaceCanvas.MapToPixelInRange(ulFinal),
            WorldSpaceCanvas.MapToPixelInRange(urFinal),
            WorldSpaceCanvas.MapToPixelInRange(llFinal),
            WorldSpaceCanvas.MapToPixelInRange(lrFinal)
        );


        ComputeShader applyShader = (ComputeShader)Resources.Load("ApplyShader");
        // Filter #1
        applyShader.SetInt("CalculationSizeX", sr.CalculationSizeX);
        applyShader.SetInt("CalculationSizeY", sr.CalculationSizeY);

        // Filter #2
        applyShader.SetInt("CalculationPositionX", sr.CalculationPositionX);
        applyShader.SetInt("CalculationPositionY", sr.CalculationPositionY);

        //UnityEngine.Debug.Log("Hit canvas at " + rakelPosition);
        //UnityEngine.Debug.Log("CalculationSizeX " + sr.CalculationSizeX);
        //UnityEngine.Debug.Log("CalculationSizeY " + sr.CalculationSizeY);


        applyShader.SetTexture(0, "Texture", target);
        //UnityEngine.Debug.Log("Dispatching with x y " + sr.ThreadGroupsX + " " + sr.ThreadGroupsY);
        applyShader.Dispatch(0, sr.ThreadGroupsX, sr.ThreadGroupsY, 1);

        // Shader
        // 1. Schauen ob in Berechnungsrange (Thread evtl. nur gespawnt wegen Gruppengröße)
        // - braucht: calculationCols, calculationRows = pixelXMax - pixelXMin + 1, pixelYMax - pixelYMin + 1
        // 2. Wenn ja, schauen ob das Pixel unter der Rakel liegt
        // - braucht:
        //   - Pixel in WorldSpace-Koordinaten
        //      - braucht:
        //          - globale Position der Shader Pixel auf dem Canvas
        //          - Transformationen für die Eckpunkte um die zurückzurechnen
        //          - xlOrigin/xrTilted
        // 3. Farbaustausch für wasauchimmer
        // - braucht: globale Position der Shader Pixel auf dem Canvas


        //Vector3 ul_ = ?;
        //Vector3 lr_ = rakelPosition - 

        //UnityEngine.Debug.Log("n_rows is " + mask.n_rows);
        //ComputeBuffer debugBuffer = new ComputeBuffer(mask.n_rows * mask.n_cols, sizeof(int));
        //int[] debugValues = new int[mask.n_rows * mask.n_cols];
        //debugBuffer.SetData(debugValues);

        //ComputeBuffer maskCoordinatesBuffer = new ComputeBuffer(mask.coordinates.GetLength(0), sizeof(int));
        //maskCoordinatesBuffer.SetData(mask.coordinates);
        //LogUtil.Log(mask.coordinates, 2, "mask");

        //ComputeShader applyShader = (ComputeShader)Resources.Load("ApplyShader");
        //applyShader.SetBuffer(0, "debug", debugBuffer);
        //applyShader.SetTexture(0, "Texture", target);
        //applyShader.SetFloat("mask_pos_x", rakelPosition.x);
        //applyShader.SetFloat("mask_pos_y", rakelPosition.y);
        //applyShader.SetFloat("mask_pos_z", rakelPosition.z); // irrelevant for now TODO though

        //applyShader.SetBuffer(0, "mask_coordinates", maskCoordinatesBuffer);
        //applyShader.SetInt("mask_y_eq_0_index", mask.y_eq_0_index);
        //applyShader.SetInt("mask_n_rows", mask.n_rows);
        //applyShader.SetInt("mask_n_cols", mask.n_cols);
        //// TODO tilt
        //// TODO rotation
        //applyShader.SetInt("mask_length", maskLength);
        //applyShader.SetInt("mask_width", maskWidth);
        ////applyShader.SetBuffer(0, "Colors", GPUBuffer);

        //applyShader.SetInt("canvas_heigth", canvasHeight);
        //applyShader.SetInt("canvas_width", canvasWidth);



        //UnityEngine.Debug.Log("dispatching for " + mask.n_cols + " " + mask.n_rows / 8);

        //applyShader.Dispatch(0, mask.n_cols, mask.n_rows / 8, 1);
        ////applyShader.Dispatch(0, target.width / 8, target.height, 1);



        //debugBuffer.GetData(debugValues);
        //LogUtil.Log(debugValues, mask.n_rows, "debug");



        //maskCoordinatesBuffer.Dispose();
        //debugBuffer.Dispose();
    }

    //public int Length { get; private set; }
    //public int Width { get; private set; }
    //private Vector2 PreviousNormal;

    //private Mask LatestMask;
    //private IMaskCalculator MaskCalculator;
    //private IMaskApplicator MaskApplicator;

    //private IRakelPaintReservoir PaintReservoir;

    //public Rakel(
    //    int length,
    //    int width,
    //    IRakelPaintReservoir paintReservoir,
    //    IMaskCalculator maskCalculator,
    //    IMaskApplicator maskApplicator)
    //{
    //    Length = length;
    //    Width = width;
    //    PaintReservoir = paintReservoir;
    //    MaskCalculator = maskCalculator;
    //    MaskApplicator = maskApplicator;
    //}

    //public void UpdateNormal(Vector2 normal, bool logMaskCalcTime = false)
    //{
    //    if (!normal.Equals(PreviousNormal))
    //    {
    //        Stopwatch sw = new Stopwatch();
    //        sw.Start();

    //        LatestMask = MaskCalculator.Calculate(Length, Width, normal);

    //        if (logMaskCalcTime)
    //        {
    //            double ns = 1000000000.0 * (double)sw.ElapsedTicks / Stopwatch.Frequency;
    //            //UnityEngine.Debug.Log("mask calc took " + ns / 1000000.0 + "ms");
    //            UnityEngine.Debug.Log("mask calc took " + ns / 1000.0 + "us");
    //        }

    //        PreviousNormal = normal;
    //    }
    //}

    //public void ApplyAt(IOilPaintSurface oilPaintSurface, Vector2Int position, bool logMaskApplyTime = false)
    //{
    //    Stopwatch sw = new Stopwatch();
    //    sw.Start();

    //    MaskApplicator.Apply(LatestMask, position, oilPaintSurface, PaintReservoir);

    //    if (logMaskApplyTime)
    //        UnityEngine.Debug.Log("mask apply took " + sw.ElapsedMilliseconds + "ms");
    //}
}
