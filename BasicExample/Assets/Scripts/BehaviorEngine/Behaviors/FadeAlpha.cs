using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class FadeAlpha : Behavior
{
    Transform m_root = null;
    float m_start = 1.0f;
    float m_end = 1.0f;
    float m_duration = 1.0f;

    public FadeAlpha(World w, string root, 
        float start, float end, float duration) : base(w)
    {
        m_start = start;
        m_end = end;
        m_duration = duration;
        m_root = world.Get(root.Trim());
    }

    public override void Setup()
    {
        base.Setup();
        m_root.gameObject.SetActive(true);

        // hack for now: shoudl get the current color and only change the alpha
        // ASN TODO: Color animators need cleanup
        world.Run(Effect());
    }

    IEnumerator Effect()
    {
        Image img = m_root.GetComponent<Image>();

        Color c = img.color;
        c.a = m_start;
        img.color = c;

        Color target = img.color;
        target.a = m_end;
        yield return ProceduralAnimator.ChangeSpriteColor(img, target, m_duration);
        m_finished = true;
    }

}
