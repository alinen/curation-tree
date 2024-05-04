using System.Collections.Generic;
using UnityEngine;

public class SequenceBehavior : ControlBehavior
{
    protected int m_current = 0;
    
    public SequenceBehavior(World w) : base(w)
    {
    }

    public override void Setup()
    {
        base.Setup();

        m_current = 0;
        m_behaviors[0].Setup();
    }

    public override void Tick()
    {
        if (!m_finished)
        {
            m_behaviors[m_current].Tick();
            if (m_behaviors[m_current].Finished())
            {
                m_behaviors[m_current].TearDown();
                m_current = m_current + 1;
                if (m_current < m_behaviors.Count)
                {
                    m_behaviors[m_current].Setup();
                }
                else
                {
                    m_finished = true;
                }
            }
        }
    }
}
