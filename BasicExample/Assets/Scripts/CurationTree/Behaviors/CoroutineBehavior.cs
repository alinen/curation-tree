using System.Collections;
using UnityEngine;

namespace CTree
{
  // Coroutine behavior 
  // Use this class to execute a coroutine as a behavior
  public class CoroutineBehavior : Behavior
  {
      IEnumerator m_enumerator;
      IEnumerator m_animation = null;

      public CoroutineBehavior(World w, IEnumerator enumerator) : base(w) 
      {
          m_enumerator = enumerator;
      }

      public override void Setup() 
      {
          base.Setup();
          if (m_animation == null) 
          {
              m_animation = Effect();
              world.Run(m_animation);
          }
      }

      public override void TearDown()
      {
          base.TearDown();
          world.Stop(m_animation);
      }

      IEnumerator Effect()
      { 
          yield return m_enumerator;
          m_finished = true;
      }

  }
}
