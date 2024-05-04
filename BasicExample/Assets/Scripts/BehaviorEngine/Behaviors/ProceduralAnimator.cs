using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public static class ProceduralAnimator
{
    public static IEnumerator Grow(Transform xform, float start, float end, float duration, bool isIcon = false)
    { 
        Vector3 position = xform.localPosition;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float u = t/duration;
            float size = Mathf.Lerp(start, end, u);
            xform.localScale = new Vector3(size, size, size);
            xform.localPosition = position; // make sure to hold xform at same place (unity bug)

            if (isIcon) LayoutRebuilder.MarkLayoutForRebuild (xform as RectTransform);
            yield return null;
        }
        xform.localPosition = position; // make sure to hold xform at same place (unity bug)
        xform.localScale = new Vector3(end, end, end);
        if (isIcon) LayoutRebuilder.MarkLayoutForRebuild (xform as RectTransform);
    }

    public static IEnumerator Pulse(Transform target, int numTimes, 
        float timePerPulseSecs = 0.4f, float pulseSize = 0.1f)
	{
		float duration = timePerPulseSecs * numTimes;
		Vector3 scale = target.localScale;
		for (float t = 0; t < duration; t+=Time.deltaTime)
		{
			float u = t/duration;
			float factor = pulseSize * Mathf.Abs(Mathf.Sin(Mathf.PI*u*numTimes));
			target.localScale = scale + new Vector3(factor, factor, 0);
			yield return null;
		}
		target.localScale = scale;
	}


    public static IEnumerator Move(Transform xform,
        Vector3 startPos, Vector3 endPos,
        float startRot, float endRot, float duration)
    {
        Quaternion startQuat = Quaternion.Euler(0.0f, 0.0f, startRot);
        Quaternion endQuat = Quaternion.Euler(0.0f, 0.0f, endRot);
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float u = t / duration;
            xform.position = Vector3.Lerp(startPos, endPos, u);
            xform.rotation = Quaternion.Lerp(startQuat, endQuat, u);
            yield return null;
        }
        xform.position = endPos;
        xform.rotation = endQuat;
    }

    public static IEnumerator Move(Transform xform,
        Vector3 startPos, Vector3 endPos, float duration)
    {
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float u = t / duration;
            xform.position = Vector3.Lerp(startPos, endPos, u);
            yield return null;
        }
        xform.position = endPos;
    }

    public static IEnumerator MoveEaseOutLocal(Transform xform,
        Vector3 startPos, Vector3 endPos, float duration)
    {
        float c1 = 1.70158f;
        float c2 = c1 + 1;
        //float c4 = (2 * Mathf.PI) / 3;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float u = t / duration;
            float uu = 1 - (1 - u) * (1 - u);
            //float uu = 1 + c2 * Mathf.Pow(u - 1, 3) + c1 * Mathf.Pow(u - 1, 2);
            //float uu = u == 0
            //? 0
            //: u == 1
            //? 1
            //: Mathf.Pow(2, -10 * u) * Mathf.Sin((u * 10 - 0.75f) * c4) + 1;
            xform.localPosition = Vector3.Lerp(startPos, endPos, uu);
            yield return null;
        }
        xform.localPosition = endPos;
    }


    // https://easings.net/en
    static float Bounce(float x)
    {
        const float n1 = 7.5625f; // 7.5625f;
        const float d1 = 2.75f;
        float u;
        if (x < 1 / d1)
        {
            u = n1 * x * x;
        }
        else if (x < 2 / d1)
        {
            u = n1 * (x -= 1.5f / d1) * x + 0.75f;
        }
        else if (x < 2.5 / d1)
        {
            u = n1 * (x -= 2.25f / d1) * x + 0.9375f;
        }
        else
        {
            u = n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
        return u;
    }

    public static IEnumerator BounceOut(Transform xform,
        Vector3 startPos, Vector3 endPos, float duration)
    {
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float x = t / duration;
            float u = Bounce(x);
            xform.position = Vector3.Lerp(startPos, endPos, u);
            yield return null;
        }
        xform.position = endPos;
    }

    public static IEnumerator BounceIn(Transform xform,
        Vector3 startPos, Vector3 endPos, float duration)
    {
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float x = t / duration;
            float u = 1.0f - Bounce(1.0f - x);
            xform.position = Vector3.Lerp(startPos, endPos, u);
            yield return null;
        }
        xform.position = endPos;
    }

    public static IEnumerator MoveLocal(Transform xform,
        Vector3 startPos, Vector3 endPos, float duration)
    {
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float u = t / duration;
            xform.localPosition = Vector3.Lerp(startPos, endPos, u);
            yield return null;
        }
        xform.localPosition = endPos;
    }

    public static IEnumerator MoveCamera(Vector3 start, Vector3 end, float duration = 1.5f)
    {
        float distance = Vector3.Distance(start, end);
        if (distance < 0.001f) yield break;

        float time = 0;
        while (time <= duration)
        {
            float mu = (1 - Mathf.Cos(time / duration * Mathf.PI)) / 2;
            Vector3 pos = Vector3.Lerp(start, end, mu);
            Camera.main.transform.position = pos;
            time += Time.deltaTime;
            yield return time;
        }
        Camera.main.transform.position = end;
    }

    public static IEnumerator MoveHud(Transform xform, Vector3 start, Vector3 end, float duration)
    {
        RectTransform rect = xform as RectTransform;
        Vector3 pos = rect.anchoredPosition;

        float time = 0;
        while (time < duration)
        {
            rect.anchoredPosition = Vector3.Lerp(start, end, time);
            time += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = end;
    }

    public static IEnumerator ShowIcon(Transform icon)
    {
        yield return Grow(icon, 0.0f, 1.0f, 1.0f, true);
        yield return Pulse(icon, 2, 1.0f, 0.1f);
    }

    public static IEnumerator FadeSprite(Transform obj,
        float startAlpha, float endAlpha, float duration)
    {
        SpriteRenderer srenderer = obj.GetComponent<SpriteRenderer>();
        if (srenderer != null)
        {
            Color c = srenderer.color;
            for (float t = 0.0f; t < duration; t += Time.deltaTime)
            {
                float u = t / duration;
                c.a = Mathf.Lerp(startAlpha, endAlpha, u);
                srenderer.color = c;
                yield return null;
            }
            c.a = endAlpha;
            srenderer.color = c;
        }
    }
    public static IEnumerator Fade(Transform obj,
        float startAlpha, float endAlpha, float duration)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float u = t / duration;
            foreach (Renderer renderer in renderers)
            {
                Color c = renderer.material.color;
                c.a = Mathf.Lerp(startAlpha, endAlpha, u);
                renderer.material.color = c;
            }
            yield return null;
        }
        foreach (Renderer renderer in renderers)
        {
            Color c = renderer.material.color;
            c.a = endAlpha;
            renderer.material.color = c;
        }
    }

    public static IEnumerator ChangeSpriteColor(Image srenderer, Color targetColor, float duration)
    {
        if (srenderer != null)
        {
            Color start = srenderer.color;
            for (float t = 0.0f; t < duration; t += Time.deltaTime)
            {
                float u = t / duration;
                Color c = Color.Lerp(start, targetColor, u);
                srenderer.color = c;
                yield return null;
            }
            srenderer.color = targetColor;
        }
    }

    public static IEnumerator ChangeColor(Transform obj, Color targetColor, float duration)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        List<Color> current = new List<Color>();
        foreach (Renderer renderer in renderers)
        {
            current.Add(renderer.material.color);
        }

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float u = t / duration;
            for (int i = 0; i < renderers.Length; i++)
            {
                Color c = Color.Lerp(current[i], targetColor, u);
                renderers[i].material.color = c;
            }
            yield return null;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = targetColor;
        }
    }

    public static void SetSpriteColor(Transform obj, Color c)
    {
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        renderer.color = c;
    }
    public static void SetColor(Transform obj, Color c)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            renderer.material.color = c;
        }
    }

    public static void SetIconSaturation(Transform icon, float v)
    {
        Material material = icon.GetComponent<Image>().material;
        material.SetFloat("_Desaturate", v);
    }

    public static IEnumerator FadeTextUI(Transform obj, float a1, float a2, float duration)
    {
        TextMeshProUGUI[] renderers = obj.GetComponentsInChildren<TextMeshProUGUI>();
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float u = t / duration;
            foreach (TextMeshProUGUI renderer in renderers)
            {
                Color c = renderer.color;
                c.a = Mathf.Lerp(a1, a2, u);
                renderer.color = c;
            }
            yield return null;
        }
        foreach (TextMeshProUGUI renderer in renderers)
        {
            Color c = renderer.color;
            c.a = a2;
            renderer.color = c;
        }
    }

    public static IEnumerator FadeText(Transform obj, float a1, float a2, float duration)
    {
        TextMeshPro[] renderers = obj.GetComponentsInChildren<TextMeshPro>();
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float u = t / duration;
            foreach (TextMeshPro renderer in renderers)
            {
                Color c = renderer.color;
                c.a = Mathf.Lerp(a1, a2, u);
                renderer.color = c;
            }
            yield return null;
        }
        foreach (TextMeshPro renderer in renderers)
        {
            Color c = renderer.color;
            c.a = a2;
            renderer.color = c;
        }
    }
    public static void SetTextColor(Transform obj, Color c)
    {
        TextMeshPro[] renderers = obj.GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro renderer in renderers)
        {
            renderer.color = c;
        }
    }

    public static IEnumerator PulseColor(Transform obj, Color c, int numTimes,
        float timePerPulseSecs = 0.4f, float pulseSize = 0.1f)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        List<Color> current = new List<Color>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer is SpriteRenderer)
            {
                SpriteRenderer srenderer = renderer as SpriteRenderer;
                current.Add(srenderer.color);
            }
            else
            {
                if (renderer.material.HasProperty("_Color"))
                {
                    current.Add(renderer.material.color);
                }
            }
        }

        float duration = timePerPulseSecs * numTimes;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float u = t / duration;
            float factor = pulseSize * Mathf.Abs(Mathf.Sin(Mathf.PI * u * numTimes));

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer is SpriteRenderer)
                {
                    SpriteRenderer srenderer = renderer as SpriteRenderer;
                    srenderer.color = Color.Lerp(current[i], c, factor);
                }
                else
                {
                    if (renderer.material.HasProperty("_Color"))
                    {
                        renderer.material.color = Color.Lerp(current[i], c, factor);
                    }
                }
            }
            yield return null;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer render = renderers[i];
            if (render is SpriteRenderer)
            {
                SpriteRenderer srender = render as SpriteRenderer;
                srender.color = current[i];
            }
            else
            {
                if (render.material.HasProperty("_Color"))
                {
                    render.material.color = current[i];
                }
            }
        }
    }

}