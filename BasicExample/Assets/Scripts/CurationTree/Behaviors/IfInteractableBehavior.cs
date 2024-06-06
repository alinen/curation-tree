using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  // Runs the given behavior is the world state variable is true
  class IfInteractableBehavior : IfBehavior
  {
      public enum Type { UNKNOWN, CLICK, DRAG, HOVER, DROP, MOUSE_OVER};

      Type m_type = Type.UNKNOWN;
      Interactable m_item = null;
      Location m_target = null;
      bool m_triggered = false;

      public IfInteractableBehavior(World w, Type t, string itemName) : 
          base(w, null)
      {
          Debug.Assert(t != Type.HOVER, "Invalid type to interactable behavior: hover");
          Debug.Assert(t != Type.DROP, "Invalid type to interactable behavior: drop");

          m_type = t;

          itemName = itemName.Trim();
          if (!string.IsNullOrEmpty(itemName))
          {
              Transform itemX = w.Get(itemName);
              Debug.Assert(itemX != null);

              if (t == Type.MOUSE_OVER)
              {
                  m_item = w.AddClickable(itemX); 
                  m_item.AddMouseOverCb(TriggerCb1);
              }
              else if (t == Type.CLICK)
              {
                  m_item = w.AddClickable(itemX); 
                  m_item.AddDragCb(TriggerCb1);
              }
              else if (t == Type.DRAG)
              {
                  m_item = w.AddDragable(itemX); 
                  m_item.AddDragCb(TriggerCb1);
              }
              m_condition = this.CheckTrigger;
          }
      }

      public IfInteractableBehavior(World w, Type t, string itemName, string targetName) : 
          base(w, null)
      {
          Debug.Assert(t != Type.MOUSE_OVER, "Invalid type in interactive behavior: mouse over");
          Debug.Assert(t != Type.DRAG, "Invalid type in interactive behavior: drag");
          Debug.Assert(t != Type.CLICK, "Invalid type in interactive behavior: click");

          m_type = t;

          itemName = itemName.Trim();
          targetName = targetName.Trim();
          if (!string.IsNullOrEmpty(itemName) && !string.IsNullOrEmpty(targetName))
          {
              Transform itemX = w.Get(itemName);
              Debug.Assert(itemX != null);
              m_item = w.AddDragable(itemX); 

              if (t == Type.HOVER)
              {
                  m_item.AddHoverCb(TriggerCb2);
              }
              else if (t == Type.DROP)
              {
                  m_item.AddDropCb(TriggerCb2);
              }

              Transform targetX = w.Get(targetName);
              Debug.Assert(targetX != null);
              m_target = w.AddLocation(targetX); 
              m_item.AddDragTarget(targetX.gameObject);

              m_condition = this.CheckTrigger;
          }
      }

      public override void Setup()
      {
          m_triggered = false;
          base.Setup();
      }

      public override void TearDown()
      {
          base.TearDown();
      }

      void TriggerCb1(Interactable source)
      {
          m_triggered = true;
      }

      void TriggerCb2(Interactable source, GameObject target)
      {
          m_triggered = (target == m_target.gameObject);
      }

      bool CheckTrigger(World w)
      {
          return m_triggered;
      }
  }
}
