using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Behavior that implements playing and stopping animations
  /// See <see cref="CTree.Factory.PlayAnimation"/>
  /// See <see cref="CTree.Factory.StopAnimation"/>
  /// </summary>
  public class Animation : Behavior
  {
      UnityEngine.Animation m_animation = null;
      string m_aniName = "";
      string m_objName = "";
      
      /// <summary>
      /// Animation mode
      /// </summary>
      public enum Mode 
      {
        /// <summary>
        /// Play the animation
        /// </summary>
        PLAY, 
        /// <summary>
        /// Stop the animation
        /// </summary>
        STOP
      };
      Mode m_mode;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="w">Object for accessing global state</param>
      /// <param name="objName">An asset with an Animation component</param>
      /// <param name="aniName">The name of an animation on <c>objName</c></param>
      /// <param name="m">Specifies whether we should play or stop the animation</param>
      public Animation(World w, string objName, string aniName, 
          Mode m = Mode.PLAY) : base(w)
      {
          m_objName = objName;
          m_aniName = aniName.Trim();
          m_mode = m;
      }

      public override void Setup()
      {
          base.Setup();

          Transform root = world.Get(m_objName.Trim()); 
          m_animation = root.GetComponent<UnityEngine.Animation>();
          m_animation[m_aniName].wrapMode = WrapMode.Loop;
          m_animation.enabled = true;
          m_animation.clip = m_animation[m_aniName].clip;

          if (m_mode == Mode.PLAY) 
          { 
              m_animation.Play();
          }
          else
          {
              m_animation.Stop();
          }
      }

      public override void Tick()
      {
          m_finished = !m_animation.isPlaying;
      }
  }
}
