using System.Collections;
using UnityEngine;

// Coroutine behavior 
// Use this class to execute a coroutine as a behavior
class CoroutineBehavior : Behavior
{
    IEnumerator m_enumerator;
    World m_w = null;
    Coroutine m_animation = null;

    public CoroutineBehavior(World w, IEnumerator enumerator) : base(w) 
    {
        m_enumerator = enumerator;
        m_w = w;
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
        yield return m_enumerator;
        m_finished = true;
    }

}
