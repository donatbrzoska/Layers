using System.Diagnostics;
using UnityEngine;

public class Stopwatch {

    static System.Diagnostics.Stopwatch sw;

    public static void Start(){
        sw = new();
        sw.Start();
    }

    public static void Stop(){
        sw.Stop();
        double ms = 1000 * (double)sw.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
        UnityEngine.Debug.Log(ms + " ms");
    }
}