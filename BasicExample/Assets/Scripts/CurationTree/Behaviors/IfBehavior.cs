using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Base class for all behaviors that trigger sub-behaviors based on a conditional.
  /// </summary>
  public class IfBehavior : ParallelBehavior
  {
      protected System.Func<World, bool> m_condition = null;
      protected bool m_isTriggered = false;

      /// <summary>
      /// Constructor 
      /// </summary>
      /// <param name="w">Object for accessing global state.</param>
      /// <param name="condition">Function that takes the World object as input and returns true or false.</param>
      public IfBehavior(World w, System.Func<World, bool> condition) : base(w)
      {
          m_condition = condition;
      }

      public override void Setup()
      {
          m_isTriggered = false;
          m_finished = false;
          m_isActive = true;
      }

      public bool IsTrue()
      {
          return m_condition(world);
      }

      public override bool Finished()
      {
          if (m_isTriggered)
          {
              return base.Finished();
          }
          return true;
      }

      public override void Tick()
      {
          if (!m_isTriggered)
          {
              m_isTriggered = m_condition(world);
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
}
