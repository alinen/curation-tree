using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace CTree
{ 
  /// <summary>
  /// Implements a central, shared object for accessing all global state.
  /// </summary>
  public class World
  {
      public bool debugRaycast = false;
      public int layerMask = 8; // interactable

      private GameLoop m_game;

      private Dictionary<int, Interactable> m_interactables = 
          new Dictionary<int, Interactable>();
      private Dictionary<int, Location> m_locations = 
          new Dictionary<int, Location>();
      private Interactable m_grabbedObject = null;
      private Interactable m_clickedObject = null;

      private Dictionary<string, Transform> m_cache = 
          new Dictionary<string, Transform>();

      private Dictionary<string,int> m_IState = new Dictionary<string, int>();
      private Dictionary<string,string> m_SState = new Dictionary<string, string>();

      public World(GameLoop game)
      {
          m_game = game;
      }

      public Transform Get(string name)
      {
          if (m_cache.ContainsKey(name))
          {
              return m_cache[name];
          }

          Transform transform = null;
          Scene scene = SceneManager.GetActiveScene();
          GameObject[] sceneObjects = scene.GetRootGameObjects();
          for (int i = 0; i < sceneObjects.Length && transform == null; i++)
          {
              GameObject obj = sceneObjects[i];
              transform = Utils.RFind(obj.transform, name);
          }
          Debug.Assert(transform != null, "Cannot find transform: "+name);

          m_cache[name] = transform;
          return transform;
      }

      public Transform Clone(Transform template, int id)
      {
          string name = template.name + id.ToString();
          if (m_cache.ContainsKey(name))
          {
              return m_cache[name];
          }

          GameObject obj = GameObject.Instantiate(template.gameObject);
          obj.name = name;
          obj.transform.parent = template.parent;

          m_cache[name] = obj.transform;
          return obj.transform;
      }

      public Coroutine Run(IEnumerator routine)
      {
          return m_game.StartCoroutine(routine);
      }

      public void Stop(IEnumerator routine)
      {
          m_game.StopCoroutine(routine);
      }

      public Interactable AddDragable(Transform obj)
      {
          return AddInteractable(obj, Interactable.Type.DRAGABLE);
      }

      public Interactable AddClickable(Transform obj)
      {
          return AddInteractable(obj, Interactable.Type.CLICKABLE);
      }

      public Interactable AddClonable(Transform obj) 
      {
          return AddInteractable(obj, Interactable.Type.COPY_DRAGABLE);
      }

      protected Interactable AddInteractable(Transform obj, Interactable.Type type) 
      {
          Interactable i = null;

          int uniqueId = obj.gameObject.GetInstanceID();
          if (m_interactables.ContainsKey(uniqueId))
          {
              //Debug.Log("Interactable already active: "+obj.name+" "+uniqueId);
              i = m_interactables[uniqueId];
              if (i.interactionType != type)
              {
                  Debug.LogWarning("Changing type if interactable: "+
                      obj.name+": type is "+i.interactionType.ToString()+
                      " and requested is "+type.ToString());
                  i.interactionType = type;
              }
          }

          i = obj.gameObject.GetComponent<Interactable>();
          if (i == null)
          {
              i = obj.gameObject.AddComponent<Interactable>();
              i.interactionType = type;
          }
          obj.gameObject.layer = layerMask; // Interactable layer
          foreach (Transform child in obj)
          {
              child.gameObject.layer = layerMask;
          }
          m_interactables[uniqueId] = i;

          // Setup drop targets
          foreach (Location location in m_locations.Values)
          {
              if (location.HasAnchor(i))
              {
                  i.AddDragTarget(location.gameObject);
              }
          }
          return i;
      }

      public Location AddLocation(Transform obj)
      {
          int uniqueId = obj.gameObject.GetInstanceID();
          if (m_locations.ContainsKey(uniqueId))
          {
              return m_locations[uniqueId];
          }

          Location loc = obj.gameObject.GetComponent<Location>();
          if (loc == null)
          {
              loc = obj.gameObject.AddComponent<Location>();
          }
          m_locations[uniqueId] = loc;

          // Setup drop targets
          foreach (Interactable i in m_interactables.Values)
          {
              if (loc.HasAnchor(i))
              {
                  i.AddDragTarget(loc.gameObject);
              }
          }
          return loc;
      }

      public void Tick()
      {
          // Ray cast for selection and drop and drop
          ClearSelected();
          RespondToPointerInput();
      }

      void RespondToPointerInput()
      {
          Transform hit = ComputeHitObject();
          if (hit != null) 
          {
              if (m_grabbedObject)
              {
                  GameObject target = null;
                  if (m_grabbedObject.CanDropOnto(hit.gameObject))
                  {
                      target = hit.gameObject;
                  }
                  if (UiDrop())
                  {
                      m_grabbedObject.OnDropTarget(target);
                      ReleaseObject();
                  }
                  else // Hover
                  {
                      m_grabbedObject.OnHoverTarget(target);
                  }
              }
              else
              {
                  Interactable mouseOverObject = hit.GetComponentInParent<Interactable>();
                  if (mouseOverObject)
                  {
                      if (!m_grabbedObject && UiPickup())
                      {
                          GrabObject(mouseOverObject);
                      }
                      mouseOverObject.OnMouseOver();
                  }
              }
          }

          if (m_grabbedObject || m_clickedObject)
          {
              if (UiDrop())
              {
                  if (m_grabbedObject) m_grabbedObject.OnDropTarget(null);
                  ReleaseObject();
              }
          }
      }

      void ReleaseObject()
      {
          if (m_clickedObject) // this is a click
          {
              if (debugRaycast) Debug.Log("SET CLICK," + m_clickedObject.name);
              m_clickedObject.SetClicked(true);
              m_clickedObject = null;
          }
          if (m_grabbedObject)
          {
              if (debugRaycast) Debug.Log("PUT_DOWN," + m_grabbedObject.gameObject.name);
              m_grabbedObject.SetGrabbed(false);
              m_grabbedObject = null;
          }
      }

      void GrabObject(Interactable obj)
      {
          if (!obj.enabled) return;

          if (obj.isDragable())
          {
              m_grabbedObject = obj;
              if (debugRaycast) Debug.Log("PICK_UP," + obj.gameObject.name);
              obj.SetGrabbed(true);
          }
          else
          {
              m_clickedObject = obj;
              if (debugRaycast) Debug.Log("CLICK," + obj.gameObject.name);
          }
      }

      // Returns active/enabled interactables currently on screen
      public List<Interactable> GetInteractables() 
      {
          List<Interactable> results = new List<Interactable>();
          foreach (KeyValuePair<int, Interactable> pair in m_interactables)
          {
              Interactable i = pair.Value;
              if (i.enabled)
              {
                  results.Add(i);
              }
          }
          return results;
      }

      void ClearSelected()
      {
          foreach (KeyValuePair<int, Interactable> pair in m_interactables)
          {
              bool grabbed = false;
              if (m_grabbedObject != null)
              {
                  if (m_grabbedObject == pair.Value)
                  {
                      grabbed = true;
                      break;
                  }
              }
              if (!grabbed)
              {
                  pair.Value.SetClicked(false);
              }
          }

      }

      Transform CheckHit(Camera camera)
      {
          Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);
          if (debugRaycast)
          {
              Debug.DrawLine(cameraRay.origin, 1000.0f*cameraRay.direction, Color.red);
          }

          RaycastHit hit;
          if (Physics.Raycast(cameraRay, out hit, 1000.0f, 1 << layerMask))
          {
              if (debugRaycast) Debug.Log("HIT: " + hit.transform.gameObject.name);
              return hit.transform;
          }
          return null;
      }

      public Transform ComputeHitObject()
      {
          Transform hit = CheckHit(Camera.main);
          return hit;
      }

      public bool UiDrop()
      {
          return Input.GetMouseButtonUp(0);
      }

      public bool UiPickup()
      {
          return Input.GetMouseButtonDown(0);
      }

      public void SetInteger(string property, int v)
      {
          m_IState[property] = v;
      }

      public int GetInteger(string property)
      {
          if (m_IState.ContainsKey(property))
          {
              return m_IState[property];
          }
          return -1;
      }

      public void SetString(string property, string v)
      {
          m_SState[property] = v;
      }

      public string GetString(string property)
      {
          if (m_SState.ContainsKey(property))
          {
              return m_SState[property];
          }
          return "";
      }        

      public override string ToString()
      {
          string contents = "";
          foreach (string key in m_SState.Keys)
          {
              string v = m_SState[key];
              contents += key + "," + v + "\n";
          }
          foreach (string key in m_IState.Keys)
          {
              int v = m_IState[key];
              contents += key + "," + v + "\n";
          }
          return contents;
      }
  }
}
