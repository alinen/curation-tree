using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Implements a control Behavior that executes all sub-behaviors in parallel.
  /// The behavior completes when all sub-behaviors have completed.
  /// </summary>
  public class ParallelBehavior : ControlBehavior
  {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="w">Object for accessing all global state.</param>
      public ParallelBehavior(World w) : base(w)
      {
      }

      public override void TearDown()
      {
          base.TearDown();
          foreach (Behavior b in m_behaviors)
          {
              b.TearDown();
          }
      }

      public override void Setup()
      {
          base.Setup();
          foreach (Behavior b in m_behaviors)
          {
              b.Setup();
          }
      }

      public override bool Finished()
      {
          foreach (Behavior b in m_behaviors)
          {
              if (!b.Finished()) return false; 
          }
          return true;
      }

      public override void Tick()
      {
          foreach (Behavior b in m_behaviors)
          {
              b.Tick();
          }
      }
  }
}
