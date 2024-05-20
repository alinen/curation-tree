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

    public override bool Finished()
    {
        return !m_isRunning && m_finished;
    }

    public override void Setup()
    {
        m_finished = false;
        m_isActive = true;

        m_isRunning = m_condition(m_w);
        if (m_isRunning) base.Setup();
    }

    public override void Tick()
    {
        if (m_isRunning) 
        {
            base.Tick();
            if (base.Finished())
            {
                Setup(); // restart sequence
            }
        }
    }
}
