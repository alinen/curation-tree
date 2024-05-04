
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;

// Factory class for creating behaviors which correspond
// to the screens that the player sees
public static class Screen
{
    // config format should be FnName:Message
    public static Behavior Create(World world, string config)
    {
        string[] mc = config.Split(':', 2);
        string fnName = mc[0];

        try
        {
            Type thisType = typeof(Screen); 
            MethodInfo theMethod = thisType.GetMethod(fnName);
            Behavior beh = theMethod.Invoke(null, new object[]{world, mc[1]}) as Behavior;
            beh.name = config; // save initializing command for debugging
            return beh;
        }
        catch(Exception e)
        {
            Debug.LogError("Cannot create behavior: "+config+" "+e.ToString());
        }
        return null;
    }

    public static Behavior FadeAlpha(World w, string config) 
    { 
        string[] tokens = config.Split(',', 4);
        string rootName = tokens[0];
        float start = 1.0f;
        float end = 1.0f;
        float duration = 1.0f;
        Single.TryParse(tokens[1], out start);
        Single.TryParse(tokens[2], out end);
        Single.TryParse(tokens[3], out duration);
        return new FadeAlpha(w, rootName, start, end, duration);
    }

    public static Behavior Grow(World w, string config) 
    { 
        string[] tokens = config.Split(',', 4);
        string rootName = tokens[0];
        float start = 1.0f;
        float end = 1.0f;
        float duration = 1.0f;
        Single.TryParse(tokens[1], out start);
        Single.TryParse(tokens[2], out end);
        Single.TryParse(tokens[3], out duration);
        return new Grow(w, rootName, start, end, duration);
    }

    public static Behavior Animation(World w, string config) 
    { 
        return new Animation(w, config);
    }

    public static Behavior Parallel(World w, string message) 
    { 
        return new ParallelBehavior(w);
    }

    public static Behavior SetState(World world, string dummy)
    {
        return new AtomicBehavior(world, (w) =>
        {
            string[] tokens = dummy.Split(',', 2);
            string key = tokens[0].Trim();
            string value = tokens[1].Trim();

            int tmp = 0;
            if (int.TryParse(value, out tmp))
            {
                w.SetInteger(key, tmp);
            }
            else
            {
                w.SetString(key, value);
            }
        });
    }

    public static Behavior Show(World world, string objName)
    {
        return new AtomicBehavior(world, (w) =>
        {
            //Debug.Log("SHOW: "+objName.Trim());
            Transform xform = world.Get(objName.Trim());
            xform.gameObject.SetActive(true);
        });
    }

    public static Behavior Hide(World world, string objName)
    {
        return new AtomicBehavior(world, (w) =>
        {
            Transform xform = world.Get(objName.Trim());
            xform.gameObject.SetActive(false);
        });
    }

    public static Behavior Sequence(World world, string dummy)
    {
        SequenceBehavior beh = new SequenceBehavior(world);
        return beh;
    }

    public static Behavior Select(World world, string args)
    {
        return new SelectBehavior(world);
    }

    public static Behavior RepeatWhile(World world, string args)
    {
        string[] tokens = args.Split(','); 
        string stateName = tokens[0].Trim();
        int stateValue;
        int.TryParse(tokens[1].Trim(), out stateValue);  
        return new RepeatBehavior(world, (world) => { 
            return world.GetInteger(stateName) == stateValue;
        });
    }

    public static Behavior Add(World world, string args)
    {
        string[] tokens = args.Split(','); 
        string stateName = tokens[0].Trim();
        int stateValue;
        int.TryParse(tokens[1].Trim(), out stateValue);  
        return new AtomicBehavior(world, (world) => { 
            int v = world.GetInteger(stateName);
            v += stateValue;
            world.SetInteger(stateName, v);
        });
    }

    public static Behavior If(World world, string args)
    {
        string[] tokens = args.Split(','); 
        string stateName = tokens[0].Trim();
        int stateValue;
        int.TryParse(tokens[1].Trim(), out stateValue);  
        return new IfBehavior(world, (world) => { 
            return world.GetInteger(stateName) == stateValue;
        }, false);
    }

    public static Behavior WaitIf(World world, string args)
    {
        string[] tokens = args.Split(','); 
        string stateName = tokens[0].Trim();
        int stateValue;
        int.TryParse(tokens[1].Trim(), out stateValue);  
        return new IfBehavior(world, (world) => { 
            return world.GetInteger(stateName) == stateValue;
        }, true);
    }

    public static Behavior WaitClick(World world, string args)
    {
        return new IfClickBehavior(world, args, true); 
    }

    public static Behavior IfClick(World world, string args)
    {
        return new IfClickBehavior(world, args, false); 
    }

    public static Behavior PlaySound(World world, string args)
    {
        return new AtomicBehavior(world, (w) =>
        {
            Transform sound = w.Get(args.Trim());
            sound.gameObject.SetActive(true);
        });
    }

    public static Behavior IfButton(World world, string args)
    {
        return new IfButtonBehavior(world, args, false);
    }

    public static Behavior WaitButton(World world, string args)
    {
        return new IfButtonBehavior(world, args, true);
    }

    public static Behavior Wait(World world, string args)
    {
        float duration = 1.0f;
        Single.TryParse(args, out duration);
        return new WaitBehavior(world, duration);
    }
}
