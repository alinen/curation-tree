using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Implements a behavior that triggers when the user interacts with scene objects 
  /// Use this behavior to support drag and drop and selection behaviors.
  /// </summary>
  public class IfInteractableBehavior : IfBehavior
  {
      /// <summary>
      /// Event types that can trigger a condition
      /// </summary>
      public enum Type { 
        /// <summary>
        /// Unknown event, e.g. the event type is not set
        /// </summary>
        UNKNOWN, 
        /// <summary>
        /// Trigger when the user clicks on an asset
        /// </summary>
        CLICK, 
        /// <summary>
        /// Trigger when the user picks up an asset
        /// </summary>
        PICKUP, 
        /// <summary>
        /// Trigger when the user drags an object on top of target object
        /// </summary>
        DRAG_ENTER, 
        /// <summary>
        /// Trigger when the user drags an object away from a target object
        /// </summary>
        DRAG_EXIT, 
        /// <summary>
        /// Trigger when the user drops an object on top of another object
        /// </summary>
        DROP, 
        /// <summary>
        /// Trigger when the player is close to an object that can be clicked or picked up
        /// </summary>
        HOVER};

      Type m_type = Type.UNKNOWN;
      Interactable m_item = null;
      Location m_target = null;
      bool m_triggered = false;

      /// <summary>
      /// Constructor for click, drag, and mouseOver events
      /// </summary>
      /// <param name="w">Object for accessing global state</param>
      /// <param name="t">The event type that will trigger the execution of sub-behaviors</param>
      /// <param name="itemName">The asset that will trigger the event.</param>
      public IfInteractableBehavior(World w, Type t, string itemName) : 
          base(w, null)
      {
          Debug.Assert(t == Type.HOVER || t == Type.PICKUP || t == Type.CLICK);
          m_type = t;

          itemName = itemName.Trim();
          if (!string.IsNullOrEmpty(itemName))
          {
              Transform itemX = w.Get(itemName);
              Debug.Assert(itemX != null);

              if (t == Type.HOVER)
              {
                  m_item = w.AddClickable(itemX); 
                  m_item.AddHoverCb(TriggerCb1);
              }
              else if (t == Type.CLICK)
              {
                  m_item = w.AddClickable(itemX); 
                  m_item.AddClickCb(TriggerCb1);
              }
              else if (t == Type.PICKUP)
              {
                  m_item = w.AddDragable(itemX); 
                  m_item.AddPickupCb(TriggerCb1);
              }
              m_condition = this.CheckTrigger;
          }
      }

      /// <summary>
      /// Constructor for hover and drop events
      /// </summary>
      /// <param name="w">Object for accessing global state</param>
      /// <param name="t">The event type that will trigger the execution of sub-behaviors</param>
      /// <param name="itemName">The asset correpsonding to the held object.</param>
      /// <param name="targetName">The asset correpsonding to the drop target.</param>
      public IfInteractableBehavior(World w, Type t, string itemName, string targetName) : 
          base(w, null)
      {
          m_type = t;

          itemName = itemName.Trim();
          targetName = targetName.Trim();
          if (!string.IsNullOrEmpty(itemName) && !string.IsNullOrEmpty(targetName))
          {
              Transform itemX = w.Get(itemName);
              Debug.Assert(itemX != null);
              m_item = w.AddDragable(itemX); 
              if (t == Type.DRAG_ENTER)
              {
                  m_item.AddDragEnterCb(TriggerCb2);
              }
              else if (t == Type.DRAG_EXIT)
              {
                  m_item.AddDragExitCb(TriggerCb2);
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
          // don't reset m_triggered until it is queried
          // this allows us to detect events that might occur when
          // the behavior is not runnning
          // so we can check for events once in base.Setup()
          base.Setup();
      }

      public override void TearDown()
      {
          base.TearDown();
      }

      void TriggerCb1(Interactable source)
      {
          Debug.Log("TriggerCb1 "+source.name+" "+name);
          m_triggered = true;
      }

      void TriggerCb2(Interactable source, GameObject target)
      {
          if (target) Debug.Log("TriggerCb2 "+source.name+" "+target.name+" "+name);
          else Debug.Log("TriggerCb2 "+source.name);

          m_triggered = (target == m_target.gameObject);
      }

      bool CheckTrigger(World w)
      {
          bool result = m_triggered;
          m_triggered = false; // clear event after it is queried
          return result;
      }
  }
}
