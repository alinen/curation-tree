using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Implements a behavior that implements highlighting and sound effects
  /// for all interactable and location objects in a scene
  /// </summary>
  public class HighlightBehavior : Behavior
  {
      AudioSource m_ssound;
      AudioSource m_lsound;
      Color m_scolor;
      Color m_lcolor;

      /// <summary>
      /// Constructor 
      /// </summary>
      /// <param name="w">Object for accessing global state</param>
      public HighlightBehavior(World w, Color sc, AudioSource ssound, 
          Color lc, AudioSource lsound) : base(w)
      {
          m_ssound = ssound;
          m_lsound = lsound;
          m_scolor = sc;
          m_lcolor = lc;
      }

      public override void Setup()
      {
         foreach (Interactable i in world.GetInteractables())
         {
             i.AddPickupCb(OnPickup);
             i.AddDropCb(OnDrop);
             i.AddHoverEnterCb(OnEnter);
             i.AddHoverExitCb(OnExit);
             i.AddDragEnterCb(OnDragEnter);
             i.AddDragExitCb(OnDragExit);
         }
      }

      void OnEnter(Interactable source)
      {
         world.Run(
            ProceduralAnimator.ChangeColor(source.transform, m_scolor, 0.03f)
         );
      }

      void OnExit(Interactable source)
      {
         world.Run(
           ProceduralAnimator.RevertColor(source.transform, 0.03f)
         );
      }

      void OnPickup(Interactable source)
      {
          m_ssound.time = 0;
          m_ssound.PlayOneShot(m_ssound.clip);
      }

      void OnDragEnter(Interactable source, GameObject target)
      {
         world.Run(
           ProceduralAnimator.ChangeColor(target.transform, m_lcolor, 0.3f)
         );
      }

      void OnDragExit(Interactable source, GameObject target)
      {
         world.Run(
           ProceduralAnimator.RevertColor(target.transform, 0.3f)
         );
      }

      void OnDrop(Interactable source, GameObject target)
      {
          if (target != null)
          {
              m_lsound.time = 0;
              m_lsound.PlayOneShot(m_lsound.clip);
          }
      }
  }
}
