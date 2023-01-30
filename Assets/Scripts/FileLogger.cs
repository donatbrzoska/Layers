using System;
using System.IO;
using UnityEngine;

public class FileLogger : MonoBehaviour
{
    string filename = "";

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void Awake()
    {
        filename = Application.dataPath + "/DebugLog.log";
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        TextWriter tw = new StreamWriter(filename, true);

        tw.WriteLine("" + logString);

        tw.Close();
    }
}
