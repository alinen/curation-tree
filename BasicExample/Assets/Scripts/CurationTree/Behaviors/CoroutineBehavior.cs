using System;
using System.Collections;
using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Implements behaviors based on Unity Coroutines.
  /// </summary>
  /// <remarks>
  /// Use this Behavior to implement animations and other effects that occur over multiple frames.
  /// Configure CoroutineBehavior with a function that takes <c>World</c> and string <c>args</c> as input and returns an iterator. 
  /// For example, the following code initializes a pause behavior with length <c>duration</c> seconds. 
  /// <code>
  /// return new CoroutineBehavior(world, args, (w, args) => {
  ///    float duration = 1.0f;
  ///    Single.TryParse(args, out duration);
  ///    return ProceduralAnimator.Wait(duration);
  /// });
  /// </code>
  /// Coroutines cannot be anonymous functions. Our Coroutines for animation behaviors are implemented 
  /// in <see cref="CTree.ProceduralAnimator?alt=ProceduralAnimator"/>.
  /// </remarks>
  public class CoroutineBehavior : Behavior
  {
      IEnumerator m_enumerator = null;
      IEnumerator m_animation = null;

      public delegate IEnumerator AnimatorFn(World b, string args);
      AnimatorFn m_fn;
      string m_args;

      /// <summary>
      /// Constructor 
      /// </summary>
      /// <param name="w">Object for accessing global state</param>
      /// <param name="enumerator">Iterator object associated with a Coroutine</param>
      public CoroutineBehavior(World w, string args, AnimatorFn enumeratorFn) : base(w) 
      {
          m_fn = enumeratorFn;
          m_args = args;
      }

      public override void Setup() 
      {
          base.Setup();

          Debug.Assert(m_animation == null);
          Debug.Assert(m_fn != null);
          m_enumerator = m_fn(world, m_args);
          m_animation = Effect();
          world.Run(m_animation);
      }

      public override void TearDown()
      {
          base.TearDown();
          world.Stop(m_animation);
          m_animation = null;
          m_enumerator = null;
      }

      IEnumerator Effect()
      { 
          yield return m_enumerator;
          m_finished = true;
      }
  }
}
