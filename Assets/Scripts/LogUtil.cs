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

    public static void Log(Color[] colors, int rows = 1, string descr = "")
    {

        if (descr != "")
        {
            Debug.Log(descr);
        }

        string result = "";
        int cols = colors.GetLength(0) / rows;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result += colors[i * cols + j].ToString() + " ";
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
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result += ints[i * cols + j].ToString() + " ";
            }
            result += "\n";
        }
        Debug.Log(result);
    }
}