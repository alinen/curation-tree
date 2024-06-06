using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CTree
{
  public class Animation : Behavior
  {
      UnityEngine.Animation m_animation = null;
      string m_aniName = "";
      public enum Mode {PLAY, STOP};
      Mode m_mode;

      public Animation(World w, string objName, string aniName, 
          bool loop = false, Mode m = Mode.PLAY) : base(w)
      {
          Transform root = world.Get(objName.Trim()); 
          m_animation = root.GetComponent<UnityEngine.Animation>();
          m_aniName = aniName.Trim();
          m_animation[m_aniName].wrapMode = WrapMode.Loop;
          m_mode = m;
      }

      public override void Setup()
      {
          base.Setup();
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
