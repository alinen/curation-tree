using System;
using System.IO;
using UnityEngine;


namespace CTree
{
  /// <summary>
  /// Logs messages to a text file stored in the persistent data path for the current platform.
  /// <remarks>
  /// Persistent data path locations are documented here:
  ///    https://docs.unity3d.com/ScriptReference/Application-dataPath.html
  /// </remarks>
  /// </summary>
  static class Logger 
  {
      public static string logfileName = "log";
      public static bool verbose = true;
      public static bool enabled = false;
      private static bool initialized = false;

      private static void Init()
      {
          string timestamp = DateTime.Now.ToString("MMddyyyHHmmss");

          logfileName = Application.dataPath + timestamp + "-" + logfileName;

          // Initialize new file
          using (StreamWriter writer = new StreamWriter(logfileName, append: false))
          {
              writer.WriteLine(timestamp + ",start");
          }
      }

      public static void Log(string msg)
      {
          if (!enabled) return;

          if (!initialized)
          {
              Init();
              initialized = true;
          }

          using (StreamWriter writer = new StreamWriter(logfileName, append: true))
          {
              string timestamp = DateTime.Now.ToString("MMddyyy-HH:mm:ss");
              writer.WriteLine(timestamp + "," + msg);
          }
      }
  }
}
