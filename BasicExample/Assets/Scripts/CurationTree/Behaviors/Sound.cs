using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
      string m_objName = "";
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
          m_objName = objName.Trim();
          m_mode = m;
      }

      public override void Setup()
      {
          base.Setup();

          Transform root = world.Get(m_objName); 
          m_sound = root.GetComponent<UnityEngine.AudioSource>();
          m_sound.time = 0;

          if (m_mode == Mode.PLAY && !m_sound.isPlaying) 
          { 
              //Debug.Log("PLAY "+m_sound.name);
              m_sound.PlayOneShot(m_sound.clip);
          }

          if (m_mode == Mode.STOP &&m_sound.isPlaying)
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
