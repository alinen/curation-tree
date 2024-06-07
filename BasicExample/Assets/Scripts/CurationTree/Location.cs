using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CTree
{
  /// <summary>
  /// Implements targets for drag-and-drop.
  /// </summary>
  /// <remarks>
  /// How the Interactable attaches to the location is determined by Transforms having the name 'Anchor'
  /// If the name of the anchor has the form <c>Anchor_InteractibleObjectName</c>, then only interactibles 
  /// matching <c>InteractibleObjectName </c>will be attached at the anchor. 
  /// If the name of the anchor is 'Anchor', then the interactible will be centered on top of the anchor
  /// </remarks>
  public class Location : MonoBehaviour
  {
      private Dictionary<string, Transform> mAnchors =
          new Dictionary<string, Transform>();

      private Dictionary<string, Interactable> mOcupants = 
          new Dictionary<string, Interactable>();

      private int mMaxOcupants = int.MaxValue;

      void Start()
      {
          Transform[] children = GetComponentsInChildren<Transform>();
          foreach (Transform child in children)
          {
              if (child.gameObject.name.Contains("Anchor"))
              {
                  int idx = child.gameObject.name.IndexOf('_');
                  if (idx != -1)
                  {
                      string key = child.gameObject.name.Substring(idx+1);
                      mAnchors[key] = child;
                  }
                  else
                  {
                      mAnchors[""] = child; 
                  }
              }
          }
          mMaxOcupants = mAnchors.Count > 0? mAnchors.Count : int.MaxValue;
      }

      void Update()
      {
          foreach (KeyValuePair<string, Interactable> pair in mOcupants)
          {
              Vector3 pos;
              Quaternion rot;
              GetAnchorPlacement(pair.Value, out pos, out rot);
              pair.Value.transform.position = pos;
              pair.Value.transform.rotation = rot;
          }
      }

      public bool IsAvailable(Interactable i)
      {
          if (i.gameObject == gameObject) return false;

          // Support function to add constraints
          bool supportedObject = mAnchors.Count == 0 || mAnchors.ContainsKey(i.gameObject.name) || mAnchors.ContainsKey("");
          bool available = (mOcupants.Count < mMaxOcupants);
          return supportedObject && available;
      }

      public void GetAnchorPlacement(Interactable i, out Vector3 pos, out Quaternion rot)
      {
          pos = Vector3.zero;
          rot = Quaternion.identity;
          if (mAnchors.ContainsKey(i.gameObject.name))
          {
              Transform anchor = mAnchors[i.gameObject.name];
              pos = anchor.position;
              rot = anchor.rotation;
          }
          else if (mAnchors.ContainsKey("")) // surface anchor
          {
              Transform anchor = mAnchors[""];
              pos = anchor.position;
              rot = anchor.rotation;
          }
          else
          {
              pos = i.transform.position;
              rot = i.transform.rotation;
          }
      }

      // Don't call this: call Interactible.AddLocation
      public void AddOcupant(Interactable i)
      {
          Debug.Assert(IsAvailable(i) == true, "ADD INVALID OCUPANT");

          mOcupants[i.gameObject.name] = i;

          // if the location has an anchor, snap to it
          Vector3 pos;
          Quaternion rot;
          GetAnchorPlacement(i, out pos, out rot);
          i.gameObject.transform.position = pos;
          i.gameObject.transform.rotation = rot;
          //Debug.Log("ADD OCUPANT: " + i.gameObject.name + " to " + gameObject.name);
      }

      // Don't call this: call Interactible.RemoveLocation
      public void RemoveOcupant(Interactable i)
      {
          mOcupants.Remove(i.gameObject.name);
          //Debug.Log("REMOVE OCUPANT: " + i.gameObject.name + " to " + gameObject.name);
      }

      public bool IsOcupant(Interactable i)
      {
          return mOcupants.ContainsKey(i.gameObject.name);
      }

  }
}
