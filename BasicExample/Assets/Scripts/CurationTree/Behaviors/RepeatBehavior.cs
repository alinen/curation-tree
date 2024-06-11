using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Implements a control Behavior that repeats all sub-behaviors until a condition becomes false.
  /// Each iteration, all sub-behaviors are run in parallel. The iteration completes when all sub-behaviors repeat.
  /// The conditional is checked at the start of each iteration. If true, the sub-behaviors are re-run.
  /// </summary>
  public class RepeatBehavior : ControlBehavior
  {
      protected System.Func<World, bool> m_condition = null;
      protected bool m_isRunning = false;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="w">Object for accessing all global state.</param>
      /// <param name="condition">Function that implements the conditional. 
      /// It takes Workd as a parameter and returns a boolean.</param>
      public RepeatBehavior(World w, System.Func<World, bool> condition) : base(w)
      {
          m_condition = condition;
      }

      public override bool Finished()
      {
          return !m_isRunning && m_finished;
      }

      public override void Setup()
      {
          base.Setup();
          m_isRunning = m_condition(world);
          if (m_isRunning)
          {
              foreach (Behavior b in m_behaviors)
              {
                  b.Setup();
              }
          }
      }

      public override void Tick()
      {
          base.Tick();
          if (m_isRunning) 
          {
              bool finished = true;
              foreach (Behavior b in m_behaviors)
              {
                  b.Tick();
                  if (b.Finished())
                  {
                     b.TearDown();
                     m_isRunning = m_condition(world);
                     if (m_isRunning) b.Setup();
                  }
                  if (!b.Finished()) finished = false;
              }
              m_finished = finished;
          }
      }
  }
}
