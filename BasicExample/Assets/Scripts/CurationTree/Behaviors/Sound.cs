using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  /// <summary>
  /// Behavior that implements playing and stopping sounds
  /// See <see cref="CTree.Factory.PlaySound"/>
  /// See <see cref="CTree.Factory.StopSound"/>
  /// </summary>
  public class Sound : Behavior
  {
      UnityEngine.AudioSource m_sound = null;
      
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
      /// <param name="objName">An asset with an AudioSource component</param>
      /// <param name="aniName">The name of a sound on <c>objName</c></param>
      /// <param name="m">Specifies whether we should play or stop the sound</param>
      public Sound(World w, string objName, Mode m = Mode.PLAY) : base(w)
      {
          Transform root = world.Get(objName.Trim()); 
          m_sound = root.GetComponent<UnityEngine.AudioSource>();
          m_mode = m;
      }

      public override void Setup()
      {
          base.Setup();
          m_sound.enabled = true;

          if (m_mode == Mode.PLAY) 
          { 
              m_sound.Play();
          }
          else
          {
              m_sound.Stop();
          }
      }

      public override void Tick()
      {
          m_finished = !m_sound.isPlaying;
      }
  }
}
