using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  // Run first behavior that has a satisfied condition
  class SelectBehavior : ControlBehavior
  {
      IfBehavior m_selected = null;

      public SelectBehavior(World w) : base(w)
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
          m_selected = null;
          base.Setup();
          foreach (Behavior b in m_behaviors)
          {
              b.Setup();
          }
      }

      public override void Tick()
      {
          if (m_selected == null)
          {
              foreach (Behavior b in m_behaviors)
              {
                  IfBehavior ifb = b as IfBehavior;
                  if (m_selected == null && ifb != null && ifb.IsTrue())
                  {
                      m_selected = ifb;
                      break;
                  }
              }

          }

          if (m_selected != null)
          { 
              m_selected.Tick();
              m_finished = m_selected.Finished();
          }
      }
  }
}
