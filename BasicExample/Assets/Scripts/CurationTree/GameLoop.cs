using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

namespace CTree 
{
  /// <summary>
  /// Unity component that implements the Curation Tree. 
  /// </summary>
  /// <remarks>
  /// Place this component on a GameObject in your scene. This component supports configuration options:
  /// * <see cref="CTree.GameLoop.Options?alt=Options"/>
  ///    * interactableLayerMask: Layer for selecting and dragging interactable objects. (Default Value: 8)
  ///    * <see cref="CTree.GameLoop.DebugOptions"/>
  ///        * tree: Display the state of the behavior tree while the game is running
  ///        * selection: Display debug information for selecting and dragging objects.
  ///    * <see cref="CTree.GameLoop.LogOptions"/>
  ///        * enabled: Toggle whether logging is enabled
  ///        * verbose: Toggle whether log text should be printed to console as well as saved to file
  ///        * baseName: Set the filename for the log. The name format is "Application.persistentDataPath/baseName-timestamp.txt"
  ///        * See <see cref="CTree.Logger?alt=Logger"/> for details.
  /// </remarks>
  public class GameLoop : MonoBehaviour
  {
      /// <summary>
      /// Debug options
      /// </summary>
      [System.Serializable]
      public class DebugOptions
      {
          public bool tree = false;
          public bool selection = false;
      }

      /// <summary>
      /// Logging options
      /// </summary>
      [System.Serializable]
      public class LogOptions
      {
          public bool enabled = false;
          public bool verbose = false;
          public string baseName = "log";
      }

      /// <summary>
      /// Configuration options
      /// </summary>
      [System.Serializable]
      public class Options
      {
          public int interactableLayerMask = 8;
          public LogOptions log = new LogOptions(); 
          public DebugOptions debug = new DebugOptions();
      }

      /// <summary>
      /// Configuration options that can be set in the Unity Editor
      /// </summary>
      public Options options = new Options();

      /// <summary>
      /// Text file for initializing the behavior tree
      /// </summary>
      public TextAsset gameConfigFile;

      SequenceBehavior m_screens = null;
      World m_world = null;

      void Start()
      {
          m_world = new World(this);
          m_world.layerMask = options.interactableLayerMask;
          if (options.log.enabled)
          {
              Logger.enabled = true;
              Logger.verbose = options.log.verbose;
              Logger.logfileName = options.log.baseName;
          }

          m_screens = new SequenceBehavior(m_world);
          Reset();
      }

      void Reset()
      {
          m_screens.Clear();
          LoadScreens(gameConfigFile);
          m_screens.Setup();
      }

      void Update()
      {
          // World State
          m_world.debugRaycast = options.debug.selection;
          m_world.Tick();

          if (m_screens.Count > 0) 
          {
              m_screens.Tick();
              if (m_screens.Finished()) // game over
              {
                  m_screens.TearDown();
                  ScriptOver();
              }
          }
      }

      void OnGUI()
      {
          if (options.debug.tree)
          {
              GUIStyle textStyle = EditorStyles.label;
              textStyle.wordWrap = true;

              GUI.color = Color.red;
              string worldState = m_world.ToString();
              string tree = PrintScreenTree(m_screens, "");
              GUILayout.Label(tree, textStyle);
              GUILayout.Label(worldState, textStyle);
          }
      }

      void PrintWorldState()
      {
          string worldState = m_world.ToString();
          Debug.Log(worldState);

          string tree = "******************\n";
          tree += PrintScreenTree(m_screens, "");
          Debug.Log(tree);
      }

      string PrintScreenTree(ControlBehavior beh, string indent)
      {
          string tree = "";
          for (int i = 0; i < beh.Count; i++)
          {
              Behavior b = beh.Get(i);
              string cname = indent + b.ToString();
              if (b.IsActive()) cname = "*" + cname;
              if (b.Finished()) cname = cname + "(f)";
              tree += cname + "\n";

              ControlBehavior cb = b as ControlBehavior;
              if (cb != null)
              {
                  tree += indent + PrintScreenTree(cb, indent + "  ");
              }
          }
          return tree;
      }

      void ScriptOver()
      {
      }

      void LoadScreens(TextAsset configFile)
      {
          char[] delim = { '\n', '\r' };
          string[] agenda = configFile.text.Split(delim);
          LoadScreens(agenda, m_screens.Count);
      }

      void LoadScreens(string[] agenda, int startId)
      {
          int screenid = startId;

          List<ControlBehavior> stack = new List<ControlBehavior>();
          ControlBehavior hierarchal = m_screens; 
          for (int i = 0; i < agenda.Length; i++)
          {
              string line = agenda[i].Trim();
              if (String.IsNullOrWhiteSpace(line)) continue;
              if (line.StartsWith("#")) continue;
              if (line.StartsWith("Quit")) break;

              if (line.StartsWith("End"))
              {
                  hierarchal = stack[stack.Count-1];
                  stack.RemoveAt(stack.Count-1);
              }
              else 
              {
                  Behavior behavior = Factory.Create(m_world, line);
                  if (behavior == null) continue;

                  if (behavior is ControlBehavior) { 
                      hierarchal.Add(behavior);
                      stack.Add(hierarchal);
                      hierarchal = behavior as ControlBehavior;
                  }
                  else 
                  {
                      hierarchal.Add(behavior);
                  }
              }
          }
          Debug.Assert(hierarchal == m_screens, "ERROR: Missing End for compound behavior");
      }
  }
}
