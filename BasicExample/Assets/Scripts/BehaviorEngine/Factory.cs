
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

    public static Behavior SetColor(World w, string config)
    {
        string[] tokens = config.Split(',', 4);
        string rootName = tokens[0];
        float r = 1.0f;
        float g = 1.0f;
        float b = 1.0f;
        Single.TryParse(tokens[1], out r);
        Single.TryParse(tokens[2], out g);
        Single.TryParse(tokens[3], out b);
        return new AtomicBehavior(w, (w) => {
            Transform obj = w.Get(rootName.Trim());
            ProceduralAnimator.SetColor(obj, new Color(r,g,b));
        });
    }

    public static Behavior Pulse(World w, string config) 
    { 
        return new CoroutineBehavior(w, config, ProceduralAnimator.Pulse); 
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

    public static Behavior IfClick(World world, string args)
    {
        return new IfClickBehavior(world, args, false); 
    }

    public static Behavior IfDrop(World world, string args)
    {
        string[] tokens = args.Split(',', 2);
        string src = tokens[0].Trim();
        string tgt = tokens[1].Trim();
        return new IfDropBehavior(world, src, tgt); 
    }

    public static Behavior IfMouseOver(World world, string args)
    {
        return new IfMouseOverBehavior(world, args); 
    }

    public static Behavior IfHover(World world, string args)
    {
        string[] tokens = args.Split(',', 2);
        string src = tokens[0].Trim();
        string tgt = tokens[1].Trim();
        return new IfHoverBehavior(world, src, tgt); 
    }

    public static Behavior IfDrag(World world, string args)
    {
        return new IfDragBehavior(world, args); 
    }

    public static Behavior Wait(World world, string args)
    {
        float duration = 1.0f;
        Single.TryParse(args, out duration);
        return new WaitBehavior(world, duration);
    }
}
