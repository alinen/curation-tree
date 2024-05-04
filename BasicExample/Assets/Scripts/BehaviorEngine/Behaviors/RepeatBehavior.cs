using System.Collections.Generic;
using UnityEngine;

// Runs a behavior sequence if the associated condition becomes true
// If "wait" is true, this behavior waits until the user triggers the condition 
public class RepeatBehavior : SequenceBehavior
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
                m_isRunning = m_condition(m_w);
                if (m_isRunning) base.Setup(); // restart sequence
                else m_finished = true;
            }
        }
    }
}
