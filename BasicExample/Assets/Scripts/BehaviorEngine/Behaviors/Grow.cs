using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grow : Behavior
{
    Transform m_root = null;
    float m_start = 1.0f;
    float m_end = 1.0f;
    float m_duration = 1.0f;
    Coroutine m_animator = null;

    public Grow(World w, string root, 
        float start, float end, float duration) : base(w)
    {
        m_start = start;
        m_end = end;
        m_duration = duration;
        m_root = world.Get(root.Trim());
    }

    public override void Setup()
    {
        base.Setup();
        m_root.gameObject.SetActive(true);
        m_animator = world.Run(Effect());
    }

    IEnumerator Effect()
    {
        yield return ProceduralAnimator.Grow(m_root, m_start, m_end, m_duration);
        m_finished = true;
    }
}
