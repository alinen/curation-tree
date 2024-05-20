using System.Collections;
using UnityEngine;

// Coroutine behavior 
// Use this class to execute a coroutine as a behavior
class CoroutineBehavior : Behavior
{
    public delegate IEnumerator UnityCoroutine(World w, string args);
    UnityCoroutine m_fn;
    World m_w = null;
    string m_args = "";
    Coroutine m_animation = null;

    public CoroutineBehavior(World w, string args, UnityCoroutine fn) : base(w) 
    {
        m_fn = fn;
        m_w = w;
        m_args = args;
    }

    public override void Setup() 
    {
        base.Setup();
        if (m_animation == null) 
        {
            m_animation = world.Run(Effect());
        }
    }

    public override void TearDown()
    {
        base.TearDown();
        m_animation = null;
    }

    IEnumerator Effect()
    { 
        yield return m_fn(m_w, m_args);
        m_finished = true;
    }

}
