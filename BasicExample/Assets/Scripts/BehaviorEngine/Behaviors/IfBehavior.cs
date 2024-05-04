using System.Collections.Generic;
using UnityEngine;

// Runs a behavior sequence if the associated condition becomes true
// If "wait" is true, this behavior waits until the user triggers the condition 
public class IfBehavior : SequenceBehavior
{
    protected System.Func<World, bool> m_condition = null;
    protected World m_w = null;
    protected bool m_wait = false;
    protected bool m_isTriggered = false;

    public IfBehavior(World w, System.Func<World, bool> condition, 
        bool wait = false) : base(w)
    {
        m_condition = condition;
        m_w = w;
        m_wait = wait;
    }

    public override void Setup()
    {
        m_isTriggered = false;
        m_finished = false;
        m_isActive = true;
    }

    public bool IsTrue()
    {
        return m_condition(m_w);
    }

    public override bool Finished()
    {
        if (m_isTriggered)
        {
            return base.Finished();
        }
        return !m_wait;
    }

    public override void Tick()
    {
        if (!m_isTriggered)
        {
            m_isTriggered = m_condition(m_w);
            if (m_isTriggered) 
            {
                base.Setup();
            }
        }
        if (m_isTriggered)
        {
            base.Tick();
        }
    }

    public override void TearDown()
    {
        if (m_isTriggered)
        {
            base.TearDown();
        }
        m_isActive = false;
    }
}
