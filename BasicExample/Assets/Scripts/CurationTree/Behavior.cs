using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace CTree
{
  /// <summary>
  /// Base class for all Behaviors.
  /// </summary>
  /// <remarks>
  /// Most custom behaviors can use the <see cref="CTree.AtomicBehavior"/> and 
  /// <see cref="CTree.CoroutineBehavior"/>. However, more complex behaviors involving 
  /// customer player interactions, such as mini games, may need to subclass Behavior.
  /// 
  /// Subclasses can override the following methods
  /// * <see cref="CTree.Behavior.Setup?alt=Setup"/>: Executed once when the Behavior starts. Use 
  /// this method to reset state or start coroutines. Overriden methods MUST call the base.Setup.
  /// * <see cref="CTree.Behavior.Tick?alt=Tick"/>: Executed every frame. 
  /// * <see cref="CTree.Behavior.TearDown?alt=TearDown"/>: Executed once when the Behavior ends. Use 
  /// this method to cleanup resources, deregister callbacks, and stop coroutines. Overriden methods MUST call the base.TearDown.
  /// * <see cref="CTree.Behavior.Finished?alt=Finished"/>: Called by GameLoop to determine the status of the currently running Behavior. 
  /// Override this method to customize the logic for determining whether a Behavior has completed.
  ///
  /// Subclasses need to set <see cref="CTree.Behavior.m_finished"/> to true to indicate that the Behavior is complete.
  ///
  /// [!Note]: This is different from a typical Behavior tree implementation where nodes return SUCCESS or FAIL 
  /// and then relinquish control back to the parent node. We may change this implementation to use canonical 
  /// behavior trees in the future. The current implementation is simpler and sufficient for our purposes where 
  /// the Curation Tree always runs once and all nodes SUCCEED.
  /// </remarks>
  public class Behavior
  {
      World m_world; 

      /// <summary>
      /// Boolean that indicates that the Behavior has finished executing and GameLoop should move to the next Behavior
      /// </summary>
      protected bool m_finished = false;

      /// <summary>
      /// Boolean that indicates that the Behavor is currently running. 
      /// Interactable objects only trigger on active Behaviors.
      /// </summary>
      protected bool m_isActive = false;
      
      private string m_name;
      private Transform m_selected = null;
      private Transform m_location = null;

      /// <summary>
      /// Value containing the input line used to create this Behavior (for debugging) 
      /// </summary>
      public virtual string name
      {
          get {  return m_name; }
          set { m_name = value; }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="w">Object for accessing global world state.</param>
      public Behavior(World w) 
      {
          m_world = w;
          m_name = GetType().ToString();
      }

      /// <summary>
      /// Override this function to perform per-frame logic.
      /// </summary>
      public virtual void Tick() { }

      /// <summary>
      /// Override this function to initializee the behavior.
      /// </summary>
      public virtual void Setup()
      {
          m_finished = false;
          m_isActive = true;
      }

      /// <summary>
      /// Override this function to cleanup the behavior.
      /// </summary>
      public virtual void TearDown()
      {
          m_isActive = false;
      }

      /// <summary>
      /// Query whether the Behavior has completed or not.
      /// </summary>
      /// <returns>True if the Behavior is finished; False otherwise.</returns>
      public virtual bool Finished()
      {
          return m_finished;
      }

      /// <summary>
      /// Query whether the Behavior is currently executing or not.
      /// </summary>
      /// <returns>True if the Behavior is currently running. False otherwise</returns>
      public bool IsActive()
      {
          return m_isActive;
      }

      /// <summary>
      /// Accessor for the object that hold global world state.
      /// </summary>
      public World world
      {
          get { return m_world; }
      }

      /// <summary>
      /// Accessor for a string representation of this Behavior.
      /// </summary>
      /// <returns>String representation for this object.</returns>
      public override string ToString()
      {
          return m_name;
      }
  }
}
