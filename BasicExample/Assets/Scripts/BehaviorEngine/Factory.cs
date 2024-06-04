
using UnityEngine;
using System;
using System.Reflection;

// Factory class for creating behaviors which correspond
// to the screens that the player sees
public static class Factory
{
    // config format should be FnName:Message
    public static Behavior Create(World world, string config)
    {
        string[] mc = config.Split(':', 2);
        string fnName = mc[0];

        try
        {
            Type thisType = typeof(Factory); 
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

    #region Control Behaviors
    public static Behavior Parallel(World w, string message) 
    { 
        return new ParallelBehavior(w);
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

    public static Behavior Repeat(World world, string args)
    {
        return new RepeatBehavior(world, (world) => { 
            return true; // run forever
        });
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

    public static Behavior IfClick(World world, string args)
    {
        return new IfInteractableBehavior(world, 
            IfInteractableBehavior.Type.CLICK, args); 
    }

    public static Behavior IfDrop(World world, string args)
    {
        string[] tokens = args.Split(',', 2);
        string src = tokens[0].Trim();
        string tgt = tokens[1].Trim();
        return new IfInteractableBehavior(world, 
            IfInteractableBehavior.Type.DROP, src, tgt); 
    }

    public static Behavior IfMouseOver(World world, string args)
    {
        return new IfInteractableBehavior(world, 
            IfInteractableBehavior.Type.MOUSE_OVER, args); 
    }

    public static Behavior IfHover(World world, string args)
    {
        string[] tokens = args.Split(',', 2);
        string src = tokens[0].Trim();
        string tgt = tokens[1].Trim();
        return new IfInteractableBehavior(world, 
            IfInteractableBehavior.Type.HOVER, src, tgt); 
    }

    public static Behavior IfDrag(World world, string args)
    {
        return new IfInteractableBehavior(world,
            IfInteractableBehavior.Type.DRAG, args); 
    }

    public static Behavior Wait(World world, string args)
    {
        float duration = 1.0f;
        Single.TryParse(args, out duration);
        return new CoroutineBehavior(world, ProceduralAnimator.Wait(duration));
    }
    #endregion

    #region Animation Behaviors
    public static Behavior PlayAnimation(World w, string config) 
    { 
        string[] tokens = config.Split(',', 2);
        string rootName = tokens[0].Trim();
        string aniName = tokens[1].Trim();
        return new Animation(w, rootName, aniName, false, Animation.Mode.PLAY);
    }

    public static Behavior LoopAnimation(World w, string config) 
    { 
        string[] tokens = config.Split(',', 2);
        string rootName = tokens[0].Trim();
        string aniName = tokens[1].Trim();
        return new Animation(w, rootName, aniName, true, Animation.Mode.PLAY);
    }

    public static Behavior StopAnimation(World w, string config) 
    { 
        string[] tokens = config.Split(',', 3);
        string rootName = tokens[0].Trim();
        string aniName = tokens[1].Trim();
        return new Animation(w, rootName, aniName, true, Animation.Mode.STOP);
    }

    #endregion

    #region Appearance Behaviors
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

    public static Behavior SetText(World world, string config)
    {
        string[] tokens = config.Split(',', 2);
        string rootName = tokens[0].Trim();
        string message = tokens[1].Trim();
        return new AtomicBehavior(world, (w) =>
        {
            Transform xform = world.Get(rootName);
            ProceduralAnimator.SetText(xform, message);
        });
    }

    public static Behavior RevertColor(World w, string config)
    {
        string[] tokens = config.Split(',', 2);
        string rootName = tokens[0];
        float d = 1.0f;
        Single.TryParse(tokens[1], out d);
        Transform obj = w.Get(rootName.Trim());
        return new CoroutineBehavior(w, ProceduralAnimator.RevertColor(obj, d));
    }

    public static Behavior ChangeColor(World w, string config)
    {
        string[] tokens = config.Split(',', 5);
        string rootName = tokens[0];
        float r = 1.0f;
        float g = 1.0f;
        float b = 1.0f;
        float d = 1.0f;
        Single.TryParse(tokens[1], out r);
        Single.TryParse(tokens[2], out g);
        Single.TryParse(tokens[3], out b);
        Single.TryParse(tokens[4], out d);
        Transform obj = w.Get(rootName.Trim());
        return new CoroutineBehavior(w, ProceduralAnimator.ChangeColor(obj, new Color(r,g,b), d));
    }

    /// Usage from script is 
    /// Pulse: TransformName (string), number of pulses (int) 
    /// </summary>
    public static Behavior Pulse(World world, string args)
    {
        string[] tokens = args.Split(',', 4);
        string rootName = tokens[0];
        int num = 1;
        float timePerPulse = 0.4f;
        float pulseSize = 0.1f;
        int.TryParse(tokens[1], out num);
        if (tokens.Length > 3) Single.TryParse(tokens[2], out timePerPulse);
        if (tokens.Length > 4) Single.TryParse(tokens[3], out pulseSize);
        Transform obj = world.Get(rootName.Trim());
        return new CoroutineBehavior(world, ProceduralAnimator.Pulse(obj, num, timePerPulse, pulseSize));
    }
    public static Behavior Move(World w, string config) 
    { 
        string[] tokens = config.Split(',', 5);
        string rootName = tokens[0];
        string startName = tokens[1];
        string endName = tokens[2];
        float duration = 1.0f;
        Single.TryParse(tokens[3], out duration);
        Transform obj = w.Get(rootName.Trim());
        Transform start = w.Get(startName.Trim());
        Transform end = w.Get(endName.Trim());

        ProceduralAnimator.Interpolator interpolator = ProceduralAnimator.Linear;
        if (tokens.Length > 4) 
        {
            if (tokens[4].StartsWith("EaseIn")) interpolator = ProceduralAnimator.EaseIn;
            else if (tokens[4].StartsWith("Cosine")) interpolator = ProceduralAnimator.Cosine;
        }

        return new CoroutineBehavior(w, ProceduralAnimator.Move(obj, start, end, duration, interpolator));
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
        Transform obj = w.Get(rootName.Trim());
        return new CoroutineBehavior(w, ProceduralAnimator.Grow(obj, start, end, duration));
    }

    public static Behavior Fade(World w, string config) 
    { 
        string[] tokens = config.Split(',', 4);
        string rootName = tokens[0];
        float start = 1.0f;
        float end = 1.0f;
        float duration = 1.0f;
        Single.TryParse(tokens[1], out start);
        Single.TryParse(tokens[2], out end);
        Single.TryParse(tokens[3], out duration);
        Transform obj = w.Get(rootName.Trim());
        return new CoroutineBehavior(w, ProceduralAnimator.Fade(obj, start, end, duration));
    }
    #endregion

    #region World State Behaviors
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
    #endregion


}
