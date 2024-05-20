using System.Collections.Generic;
using UnityEngine;

// Runs a set of behaviors in paralle until the associated condition becomes false
public class RepeatBehavior : ParallelBehavior
{
    protected System.Func<World, bool> m_condition = null;
    protected World m_w = null;
    protected bool m_isRunning = false;

    public RepeatBehavior(World w, System.Func<World, bool> condition) : base(w)
    {
        m_condition = condition;
        m_w = w;
    }

    public override void Setup()
    {
        m_isRunning = m_condition(m_w);
        if (m_isRunning) base.Setup();
        else m_finished = true;
    }

    public override void Tick()
    {
        if (m_isRunning) 
        {
            base.Tick();
            if (base.Finished())
            {
                base.Setup(); // restart sequence
            }
        }
    }
}
