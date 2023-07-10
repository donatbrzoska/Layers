using System.Collections.Generic;
using UnityEngine;

public class LogUtil
{
    private LogUtil(){ }

    public static void Log(List<Vector2Int> vs, string descr = "")
    {
        if (descr != "")
        {
            Debug.Log(descr);
        }

        foreach (Vector2Int v in vs)
        {
            Debug.Log(v);
        }
    }

    public static void Log(List<Vector2> vs, string descr = "")
    {
        if (descr != "")
        {
            Debug.Log(descr);
        }

        foreach (Vector2 v in vs)
        {
            Debug.Log(v);
        }
    }

    public static void Log(bool[,] arr, string descr = "")
    {
        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                if (arr[i, j] == true)
                {
                    result += " 1";
                }
                else
                {
                    result += " 0";
                }
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    public static void Log(int[,] arr, string descr = "")
    {
        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                result += arr[i, j] + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    public static void Log(Vector4[] values, Vector3Int dimensions, int usedDepth, DebugListType debugElementType, string descr)
    {
        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        for (int row = dimensions.y - 1; row >= 0; row--)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                result += "[";
                for (int k = 0; k < usedDepth; k++)
                {
                    int index = IndexUtil.XYZ(col, row, k, dimensions);
                    string color_str = Float4ToString(values[index], debugElementType);
                    result += color_str;
                    if (k < usedDepth - 1)
                    {
                        result += " ";
                    }
                }
                result += "]  ";
            }
            result += "\n\n";
        }
        Debug.Log(result);
    }

    public static void Log(Paint[] values, Vector3Int dimensions, int usedDepth, string descr = "")
    {
        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        for (int row = dimensions.y - 1; row >= 0; row--)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                result += "[";
                for (int k = 0; k < usedDepth; k++)
                {
                    int index = IndexUtil.XYZ(col, row, k, dimensions);
                    result += values[index].ToString();
                    if (k < usedDepth - 1)
                    {
                        result += " ";
                    }
                }
                result += "]  ";
            }
            result += "\n\n";
        }
        Debug.Log(result);
    }

    public static void Log(ColumnInfo[] values, Vector3Int dimensions, int usedDepth, string descr = "")
    {
        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        for (int row = dimensions.y - 1; row >= 0; row--)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                result += "[";
                for (int k = 0; k < usedDepth; k++)
                {
                    int index = IndexUtil.XYZ(col, row, k, dimensions);
                    result += values[index].ToString();
                    if (k < usedDepth - 1)
                    {
                        result += " ";
                    }
                }
                result += "]  ";
            }
            result += "\n\n";
        }
        Debug.Log(result);
    }

    private static string Float4ToString(Vector4 value, DebugListType debugType)
    {
        string result = "(";
        for (int i=0; i<(int)debugType; i++)
        {
            string format = "F6";
            switch(i)
            {
                case 0:
                    result += value.x.ToString(format);
                    break;
                case 1:
                    result += value.y.ToString(format);
                    break;
                case 2:
                    result += value.z.ToString(format);
                    break;
                case 3:
                    result += value.w.ToString(format);
                    break;
                default:
                    break;
            }
            if (i<(int)debugType - 1) // This assumes that DebugListType.None exists!
            {
                result += ", ";
            }
        }
        result += ")";
        return result;
    }

    public static void Log(Vector3[] vecs, int rows = 1, string descr = "")
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = vecs.GetLength(0) / rows;
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                result += vecs[i * cols + j].ToString("F2") + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    public static void Log(Vector3Int[] vecs, int rows = 1, string descr = "")
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = vecs.GetLength(0) / rows;
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                result += vecs[i * cols + j].ToString() + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    public static void Log(Vector2[] vecs, int rows = 1, string descr = "")
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = vecs.GetLength(0) / rows;
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                result += vecs[i * cols + j].ToString("F2") + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    public static void Log(Vector2Int[] vecs, int rows = 1, string descr = "")
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = vecs.GetLength(0) / rows;
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                result += vecs[i * cols + j].ToString() + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    public static void Log(int[] ints, int rows = 1, bool readable = true, string descr = "")
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = ints.GetLength(0) / rows;

        if (readable)
        {
            for (int i = rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < cols; j++)
                {
                    result += string.Format("{0,6:D}", ints[i * cols + j]) + ", ";
                }
                result += "\n";
            }
        }
        else
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result += string.Format("{0,6:D}", ints[i * cols + j]) + ", ";
                }
                result += "\n";
            }
        }
        Debug.Log(result);
    }

    public static void Log(float[] floats, int rows = 1, bool readable = true, string descr = "")
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = floats.GetLength(0) / rows;

        if (readable)
        {
            for (int i = rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < cols; j++)
                {
                    result += string.Format("{0,6:0.000}", floats[i * cols + j]) + ", ";
                }
                result += "\n";
            }
        }
        else
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result += string.Format("{0,6:0.000}f", floats[i * cols + j]) + ", ";
                }
                result += "\n";
            }
        }
        Debug.Log(result);
    }

    // This function is for printing a specified z of a 3D array
    public static void LogVolumes(Paint[] paints, int rows, int cols, int z, string descr = "")
    {
        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result += paints[z * rows * cols + i * cols + j].Volume + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }
}