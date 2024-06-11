using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Base class for all behaviors that trigger sub-behaviors based on a conditional.
  /// </summary>
  public class IfBehavior : ControlBehavior
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
          base.Setup();
          m_isTriggered = m_condition(world);
          if (m_isTriggered) 
          {
              foreach (Behavior b in m_behaviors)
              {
                  b.Setup();
              }
          }
          else
          {
              m_finished = true;
          }
      }

      public bool IsTrue()
      {
          return m_condition(world);
      }

      public override void Tick()
      {
          base.Tick();
          if (m_isTriggered)
          {
              bool finished = true;
              foreach (Behavior b in m_behaviors)
              {
                 b.Tick();
                 if (!b.Finished()) finished = false;
              }
              m_finished = finished;
          }
      }

      public override void TearDown()
      {
          base.TearDown();
          if (m_isTriggered)
          {
              foreach (Behavior b in m_behaviors)
              {
                 b.TearDown();
              }
          }
      }
  }
}
