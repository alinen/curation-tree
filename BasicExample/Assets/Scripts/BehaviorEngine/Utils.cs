using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// Common utilities
public static class Utils
{
    private static System.Random m_random = new System.Random();

    static public int Range(int a, int b)
    {
        return m_random.Next() % (b - a) + a;
    }

    static public float Range(float a, float b)
    {
        return (float) m_random.NextDouble() * (b - a) + a;
    }


    static public int Rand() // integer between 0 and RAND_MAX
    {
        return m_random.Next();
    }

    static public float RandFloat()
    {
        return (float)m_random.NextDouble();
    }

    static public float Exp(float lambda)
    {
        if (lambda == 0) return 0;
        float u = (float)m_random.NextDouble();
        float x = (float)Mathf.Log(1 - u) / -lambda;
        return x;
    }

    // recursive find for transform equalling the given name
    public static string MakeString(List<string> list)
    {
        if (list.Count == 0) return "[]";

        string result = "['" + list[0];
        for (int i = 1; i < list.Count; i++) 
        {
            result += "','" + list[i];
        }
        result += "]";
        return result;
    }

    public static Transform RFind(Transform root, string name)
    {
        if (root.name == name)
        {
            return root;
        }

        Transform found = null;
        for (int i = 0; i < root.childCount && found == null; i++)
        {
            Transform child = root.GetChild(i);
            found = RFind(child, name);
        }
        return found;
    }

    // by toothycat, http://answers.unity3d.com/questions/143597/font-metrics.html
    // ASN, modified to use font size
    public static void SetAlpha(Transform xform, float alpha)
    {
        SpriteRenderer[] renderers = xform.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            Color c = renderer.color;
            c.a = alpha;
            renderer.color = c;
        }
    }

    public static float Dot(List<float> a, List<float> b)
    {
        float sum = 0;
        for (int i = 0; i < a.Count; i++) 
        {
            sum += a[i] * b[i];
        }
        return sum;
    }
}
