using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class GameLoop : MonoBehaviour
{
    public Transform env;
    public Transform hud;
    public TextAsset gameConfigFile;
    public int interactableLayerMask = 8;
    public bool debug = false;
    public bool debugRaycast = false;

    protected SequenceBehavior m_screens = null;
    protected World m_world = null;

    void Start()
    {
        m_world = new World(this);
        m_world.layerMask = interactableLayerMask;

        m_screens = new SequenceBehavior(m_world);
        Reset();
    }

    protected virtual void Reset()
    {
        m_screens.Clear();
        LoadScreens(gameConfigFile);
        m_screens.Setup();
    }

    void Update()
    {
        if (m_screens.Count == 0) return;

        // World State
        m_world.debugRaycast = debugRaycast;

        m_world.Tick();
        m_screens.Tick();
        if (m_screens.Finished()) // game over
        {
            m_screens.TearDown();
            ScriptOver();
        }
    }

    void OnGUI()
    {
        if (debug)
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

    protected void PrintWorldState()
    {
        string worldState = m_world.ToString();
        Debug.Log(worldState);

        string tree = "******************\n";
        tree += PrintScreenTree(m_screens, "");
        Debug.Log(tree);
    }

    protected string PrintScreenTree(ControlBehavior beh, string indent)
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

    protected virtual void ScriptOver()
    {
    }

    protected virtual void LoadScreens(TextAsset configFile)
    {
        char[] delim = { '\n', '\r' };
        string[] agenda = configFile.text.Split(delim);
        LoadScreens(agenda, m_screens.Count);
    }

    protected virtual void LoadScreens(string[] agenda, int startId)
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