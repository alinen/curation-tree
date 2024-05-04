using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public string logfileName = "log";
    public bool verbose = true;

    void Start()
    {
        string timestamp = DateTime.Now.ToString("MMddyyyHHmmss");
        logfileName = timestamp + "-" + logfileName;

        // Initialize new file
        using (StreamWriter writer = new StreamWriter(logfileName, append: false))
        {
            writer.WriteLine(timestamp + ",start");
        }
    }

    public void Log(string msg)
    {
        using (StreamWriter writer = new StreamWriter(logfileName, append: true))
        {
            string timestamp = DateTime.Now.ToString("MMddyyy-HH:mm:ss");
            writer.WriteLine(timestamp + "," + msg);
        }
    }
}
