using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : Behavior
{
    Transform root = null;
    UnityEngine.Animation animation = null;

    public Animation(World w, string config) : base(w)
    {
        root = world.Get(config.Trim()); 
        animation = root.GetComponent<UnityEngine.Animation>();
    }

    public override void Setup()
    {
        base.Setup();
        root.gameObject.SetActive(true);
        animation.enabled = true;
        animation.Play();
    }

    public override void Tick()
    {
        m_finished = !animation.isPlaying;
    }
}
