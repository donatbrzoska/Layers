using System.Collections.Generic;
using UnityEngine;

public enum DebugType
{
    None,
    Float,
    Float2,
    Float3,
    Float4
}

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

    public static void Log(Color[] colors, int rows = 1, string descr = "", DebugType debugType = DebugType.Float4)
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = colors.GetLength(0) / rows;
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                //string color_str = colors[i * cols + j].rToString();
                string color_str = ColorToString(colors[i * cols + j], debugType);
                result += color_str + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    private static string ColorToString(Color color, DebugType debugType)
    {
        string result = "(";
        for (int i=0; i<(int)debugType; i++)
        {
            result += color[i].ToString("F2");
            if (i<(int)debugType - 1)
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

    public static void Log(int[] ints, int rows = 1, string descr = "")
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = ints.GetLength(0) / rows;
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                result += ints[i * cols + j].ToString() + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    public static void Log(float[] floats, int rows = 1, string descr = "")
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = floats.GetLength(0) / rows;
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                result += floats[i * cols + j].ToString("F2") + " ";
            }
            result += "\n";
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