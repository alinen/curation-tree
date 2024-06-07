using System.Collections;
using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Implements behaviors that perform actions implemented with a Unity Coroutine.
  /// </summary>
  /// <remarks>
  /// Use this Behavior to implement animations and other effects that occur over multiple frames.
  /// Configure CoroutineBehavior with a function that takes <c>World</c> as input and returns an iterator. 
  /// For example, the following code increases the size of <c>xform</c> to twice its original size over 3.0 seconds. 
  /// <code>
  /// Transform xform = world.Get(objName.Trim());
  /// Behavior b = new CoroutineBehavior(world, 
  ///      ProceduralAnimator.Grow(xform, 1, 2, 3.0));
  /// </code>
  /// Coroutines cannot be anonymous functions. Our Coroutines for animation behaviors are implemented 
  /// in <see cref="CTree.ProceduralAnimator?alt=ProceduralAnimator"/>.
  /// </remarks>
  public class CoroutineBehavior : Behavior
  {
      IEnumerator m_enumerator;
      IEnumerator m_animation = null;


      /// <summary>
      /// Constructor 
      /// </summary>
      /// <param name="w">Object for accessing global state</param>
      /// <param name="enumerator">Iterator object associated with a Coroutine</param>
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
