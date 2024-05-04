using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitBehavior : Behavior
{
    float m_duration = 1.0f;
    public WaitBehavior(World w, float duration) : base(w) 
    {
        m_duration = duration;
    }

    public override void Setup()
    {
        base.Setup();
        world.Run(m_Wait());
    }

    IEnumerator m_Wait()
    {
        yield return new WaitForSeconds(m_duration);
        m_finished = true;
    }
}
