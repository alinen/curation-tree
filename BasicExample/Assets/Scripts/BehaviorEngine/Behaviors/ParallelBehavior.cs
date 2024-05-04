using System.Collections.Generic;
using UnityEngine;

class ParallelBehavior : ControlBehavior
{
    public ParallelBehavior(World w) : base(w)
    {
    }

    public override void TearDown()
    {
        base.TearDown();
        foreach (Behavior b in m_behaviors)
        {
            b.TearDown();
        }
    }

    public override void Setup()
    {
        base.Setup();
        foreach (Behavior b in m_behaviors)
        {
            b.Setup();
        }
    }

    public override bool Finished()
    {
        foreach (Behavior b in m_behaviors)
        {
            if (!b.Finished()) return false; 
        }
        return true;
    }

    public override void Tick()
    {
        foreach (Behavior b in m_behaviors)
        {
            b.Tick();
        }
    }
}
