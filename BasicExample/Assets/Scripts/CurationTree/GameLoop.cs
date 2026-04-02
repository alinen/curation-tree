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
      public class TreeVizOptions
      {
          public bool enabled = false;
          public int fontSize = 20;
          public float widthPercent = 0.25f;
      }

      [System.Serializable]
      public class DebugOptions
      {
          public TreeVizOptions tree; 
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

#if UNITY_EDITOR
      Vector2 scrollPosition;
      private Texture2D MakeTex(int width, int height, Color col)
      {
         Color[] pix = new Color[width * height];
         for (int i = 0; i < pix.Length; ++i)
         {
            pix[i] = col;
         }
         Texture2D result = new Texture2D(width, height);
         result.SetPixels(pix);
         result.Apply();
         return result;
      }
        
      void OnGUI()
      {
          if (options.debug.tree.enabled)
          {
              GUIStyle textStyle = new GUIStyle(EditorStyles.label);
              int fontSize = textStyle.fontSize;
              textStyle.fontSize = options.debug.tree.fontSize;
              textStyle.richText = true;
              textStyle.wordWrap = false;
              textStyle.fixedWidth = Screen.width;
              textStyle.normal.background =MakeTex(1,1, new Color(0,0,0,0.75f)); 
              
              float width = Screen.width * options.debug.tree.widthPercent;
              scrollPosition = GUILayout.BeginScrollView(
                  scrollPosition, GUILayout.Width(width), GUILayout.Height(Screen.height)); 

              string tree = PrintScreenTree(m_screens, "");
              GUILayout.Label(tree, textStyle);

              string worldState = m_world.ToString();
              GUILayout.Label(worldState, textStyle);
              GUILayout.EndScrollView();
          }
      }
#endif

      void PrintWorldState()
      {
          string worldState = m_world.ToString();
          Debug.Log(worldState);
      }

      string PrintScreenTree(ControlBehavior beh, string indent)
      {
          string tree = "";
          for (int i = 0; i < beh.Count; i++)
          {
              Behavior b = beh.Get(i);
              string cname = indent + b.ToString();
              if (b.IsActive()) cname = "<color=#00ff00ff>*" + cname + "</color>";
              else if (b.Finished()) cname = "<color=#c0c0c0c0>" + cname + " (f) </color>";
              else cname = "<b>" + cname + "</b>";
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
