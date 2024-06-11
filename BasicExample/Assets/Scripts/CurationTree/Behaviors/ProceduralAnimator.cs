using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using System.Linq;

namespace CTree
{
  /// <summary>
  /// Utility functions for animating assets
  /// </summary>
  public static class ProceduralAnimator
  {

      /// <summary>
      /// Wait for given number of seconds
      /// </summary>
      /// <param name="duration">The amount of time to pause in seconds</param>
      public static IEnumerator Wait(float duration)
      {
          for (float t = 0.0f; t < duration; t += Time.deltaTime)
          { 
             yield return null;
          }
      }

      /// <summary>
      /// Animate the given transform with a uniform scale.
      /// </summary>
      /// <param name="xform">The transform to scale. The scale applies to all children.</param>
      /// <param name="start">The starting size in local scene units, e.g. with respect to the parent of xform.</param>
      /// <param name="end">The ending size in local scene units, e.g. with respect to the parent of xform.</param>
      /// <param name="duration">The length of the animation (in seconds)</param>
      public static IEnumerator Grow(Transform xform, float start, float end, float duration)
      { 
          // Set isIcon to true if the transform is part of the UI
          bool isIcon = xform.GetComponent<RectTransform>() != null; 

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

      /// <summary>
      /// Animate the given transform with a pulse in size.
      /// </summary>
      /// <param name="target">The transform to pulse. The pulse applies to all children.</param>
      /// <param name="numTimes">The number of times to pulse.</param>
      /// <param name="timePerPulseSecs">The duration of each pulse (in seconds)</param>
      /// <param name="pulseSize">How much to change the size (percentage of original xform size)</param>
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

      /// <summary>
      /// Set the contents of the text component on the given transform
      /// </summary>
      /// <param name="xform">The transform to update.</param>
      /// <param name="message">The text to set.</param>
      public static void SetText(Transform xform, string message)
      {
          TextMeshProUGUI tgui = xform.GetComponent<TextMeshProUGUI>();
          TextMeshPro tpro = xform.GetComponent<TextMeshPro>();
          if (tgui != null) tgui.text = message; 
          if (tpro != null) tpro.text = message; 
      }

  #region Spatial Interpolators

      /// <summary>
      /// Implementaton function for normalizing time into the range [0, 1] given a desired duration.
      /// </summary>
      /// <param name="t">Value between 0 and duration, inclusive</param>
      /// <param name="duration">Duration of the interpolation (seconds)</param>
      /// <returns>Normalized value between 0 and 1</returns>
      public delegate float Interpolator(float t, float duration);

      /// <summary>
      /// Implements a normalized time for linear interpolation. 
      /// </summary>
      /// <param name="t">Value between 0 and duration, inclusive</param>
      /// <param name="duration">Duration of the interpolation (seconds)</param>
      /// <returns>The value (t / duration)</returns>
      public static float Linear(float t, float duration)
      {
          return (t / duration);
      }

      /// <summary>
      /// Implements a normalized time for cosine interpolation
      /// </summary>
      /// <param name="t">Value between 0 and duration, inclusive</param>
      /// <param name="duration">Duration of the interpolation (seconds)</param>
      /// <returns>The value (1 - Mathf.Cos(t / duration * Mathf.PI)) / 2</returns>
      public static float Cosine(float t, float duration)
      {
          float mu = (1 - Mathf.Cos(t / duration * Mathf.PI)) / 2;
          return mu;
      }

      /// <summary>
      /// Implements a normalized time for ease in interpolation. 
      /// </summary>
      /// <param name="t">Value between 0 and duration, inclusive</param>
      /// <param name="duration">Duration of the interpolation (seconds)</param>
      /// <returns>The value (1 - (1 - t / duration)^2)</returns>
      // https://easings.net/en
      public static float EaseIn(float t, float duration)
      {
          float u = t / duration;
          float uu = 1 - (1 - u) * (1 - u);
          return uu;
      }

      /// <summary>
      /// Move and rotate xform with a bounce at the <c>start</c> and <c>end</c>
      /// </summary>
      /// <param name="xform">The transform to animate. Applies to all children.</param>
      /// <param name="start">The starting position and rotation.</param>
      /// <param name="end">The ending position and rotation.</param>
      /// <param name="duration">The length of the animation (in seconds)</param>
      /// <param name="interpolator">The interpolation method to use.</param>
      /// <returns> None </returns>
      public static IEnumerator Move(Transform xform,
          Transform start, Transform end, float duration, Interpolator interpolator)
      {
          for (float t = 0.0f; t < duration; t += Time.deltaTime)
          {
              float u = interpolator(t, duration);
              xform.position = Vector3.Lerp(start.position, end.position, u);
              xform.rotation = Quaternion.Lerp(start.rotation, end.rotation, u);
              yield return null;
          }
          xform.position = end.position;
          xform.rotation = end.rotation;
      }
  #endregion

  #region Color Interpolation

      /// <exclude/>
      static private Dictionary<object, Color> g_colorStash = new Dictionary<object, Color>();

      /// <summary>
      /// Change the value of alpha for all colors associated with the given transform (including children)
      /// Linearly interpolates between the <c>start</c> and <c>end</c> values. 
      /// </summary>
      /// <param name="obj">The transform to change alpha.</param>
      /// <param name="startAlpha">The starting alpha.</param>
      /// <param name="endAlpha">The ending alpha.</param>
      /// <param name="duration">The length of the animation (in seconds)</param>
      public static IEnumerator Fade(Transform obj,
          float startAlpha, float endAlpha, float duration)
      {
          Renderer[] meshes = obj.GetComponentsInChildren<Renderer>();
          TextMeshProUGUI[] tguis = obj.GetComponentsInChildren<TextMeshProUGUI>();
          TextMeshPro[] tpros = obj.GetComponentsInChildren<TextMeshPro>();
          Image[] imgs = obj.GetComponentsInChildren<Image>();

          for (float t = 0.0f; t < duration; t += Time.deltaTime)
          {
              float u = t / duration;
              float a = Mathf.Lerp(startAlpha, endAlpha, u);
              foreach (Renderer r in meshes)
              {
                  Color c = r.material.color;
                  c.a = a;
                  r.material.color = c;
              }
              foreach (TextMeshProUGUI r in tguis)
              {
                  Color c = r.color;
                  c.a = a;
                  r.color = c;
              }
              foreach (TextMeshPro r in tpros)
              {
                  Color c = r.color;
                  c.a = a;
                  r.color = c;
              }
              foreach (Image r in imgs)
              {
                  Color c = r.color;
                  c.a = a;
                  r.color = c;
              }
              yield return null;
          }
      }

      /// <summary>
      /// Change the color for all renderers associated with the given transform (including children)
      /// Linearly interpolates between the current color on <c>obj</c> and <c>end</c> colors. 
      /// </summary>
      /// <param name="obj">The transform to change alpha.</param>
      /// <param name="end">The ending color.</param>
      /// <param name="duration">The length of the animation (in seconds)</param>
      public static IEnumerator ChangeColor(Transform obj, Color end, float duration)
      {
          Debug.Log("CHANGE COLOR: "+obj.name);
          Renderer[] meshes = obj.GetComponentsInChildren<Renderer>();
          TextMeshProUGUI[] tguis = obj.GetComponentsInChildren<TextMeshProUGUI>();
          TextMeshPro[] tpros = obj.GetComponentsInChildren<TextMeshPro>();
          Image[] imgs = obj.GetComponentsInChildren<Image>();

          Dictionary<object, Color> localStash = new Dictionary<object, Color>();
          for (float t = 0.0f; t < duration; t += Time.deltaTime)
          {
              float u = t / duration;
              foreach (Renderer r in meshes)
              {
                  if (!g_colorStash.ContainsKey(r)) g_colorStash[r] = r.material.color;
                  if (!localStash.ContainsKey(r)) localStash[r] = r.material.color;
                  Color c = Color.Lerp(localStash[r], end, u);
                  r.material.color = c;
              }
              foreach (TextMeshProUGUI r in tguis)
              {
                  if (!g_colorStash.ContainsKey(r)) g_colorStash[r] = r.color;
                  if (!localStash.ContainsKey(r)) localStash[r] = r.color;
                  Color c = Color.Lerp(localStash[r], end, u);
                  r.color = c;
              }
              foreach (TextMeshPro r in tpros)
              {
                  if (!g_colorStash.ContainsKey(r)) g_colorStash[r] = r.color;
                  if (!localStash.ContainsKey(r)) localStash[r] = r.color;
                  Color c = Color.Lerp(localStash[r], end, u);
                  r.color = c;
              }
              foreach (Image r in imgs)
              {
                  if (!g_colorStash.ContainsKey(r)) g_colorStash[r] = r.color;
                  if (!localStash.ContainsKey(r)) localStash[r] = r.color;
                  Color c = Color.Lerp(localStash[r], end, u);
                  r.color = c;
              }
              yield return null;
          }
      }

      /// <summary>
      /// Reverts the color for all renderers associated with the given transform (including children)
      /// Linearly interpolates between the current color on <c>obj</c> and the original color.
      /// </summary>
      /// <param name="obj">The transform to change alpha.</param>
      /// <param name="duration">The length of the animation (in seconds)</param>
      public static IEnumerator RevertColor(Transform obj, float duration)
      {
          Debug.Log("REVERT: "+obj.name);
          Renderer[] meshes = obj.GetComponentsInChildren<Renderer>();
          TextMeshProUGUI[] tguis = obj.GetComponentsInChildren<TextMeshProUGUI>();
          TextMeshPro[] tpros = obj.GetComponentsInChildren<TextMeshPro>();
          Image[] imgs = obj.GetComponentsInChildren<Image>();

          Dictionary<object, Color> localStash = new Dictionary<object, Color>();
          for (float t = 0.0f; t < duration; t += Time.deltaTime)
          {
              float u = t / duration;
              foreach (Renderer r in meshes)
              {
                  if (g_colorStash.ContainsKey(r)) 
                  {
                      if (!localStash.ContainsKey(r)) localStash[r] = r.material.color;
                      Color c = Color.Lerp(localStash[r], g_colorStash[r], u);
                      r.material.color = c;
                  }
              }
              foreach (TextMeshProUGUI r in tguis)
              {
                  if (g_colorStash.ContainsKey(r))
                  {
                      if (!localStash.ContainsKey(r)) localStash[r] = r.color;
                      Color c = Color.Lerp(localStash[r], g_colorStash[r.name], u);
                      r.color = c;
                  }
              }
              foreach (TextMeshPro r in tpros)
              {
                  if (g_colorStash.ContainsKey(r))
                  {
                      if (!localStash.ContainsKey(r)) localStash[r] = r.color;
                      Color c = Color.Lerp(localStash[r], g_colorStash[r], u);
                      r.color = c;
                  }
              }
              foreach (Image r in imgs)
              {
                  if (g_colorStash.ContainsKey(r))
                  {
                      if (!localStash.ContainsKey(r)) localStash[r] = r.color;
                      Color c = Color.Lerp(localStash[r], g_colorStash[r], u);
                      r.color = c;
                  }
              }
              yield return null;
          }
      }
      #endregion
  }
}
