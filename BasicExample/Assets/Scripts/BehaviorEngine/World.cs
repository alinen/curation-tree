using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using static UnityEditor.Progress;

// note: Screens can find assets in three places:
//   hud: UI elements such as the journal and outcome bar
//   environment: 3D scene root and objects
public class World
{
    public bool debugRaycast = false;

    protected GameLoop m_game;

    private Dictionary<int, Interactable> m_interactables = 
        new Dictionary<int, Interactable>();
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

        Transform transform = Utils.RFind(m_game.hud, name);
        if (transform == null) transform = Utils.RFind(m_game.env, name);
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

    public Interactable AddInteractable(Transform obj)
    {
        int uniqueId = obj.gameObject.GetInstanceID();
        if (m_interactables.ContainsKey(uniqueId))
        {
            //Debug.Log("Interactable already active: "+obj.name+" "+uniqueId);
            return m_interactables[uniqueId];
        }

        Interactable i = obj.gameObject.GetComponent<Interactable>();
        if (i == null)
        {
            i = obj.gameObject.AddComponent<Interactable>();
        }
        obj.gameObject.layer = 8; // Interactable
        foreach (Transform child in obj)
        {
            child.gameObject.layer = 8;
        }
        m_interactables[uniqueId] = i;
        return i;
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
            //Debug.Log("HIT: "+hit.name);
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
            //Debug.Log("SET CLICK," + m_clickedObject.name);
            m_clickedObject.SetClicked(true);
            m_clickedObject = null;
        }
        if (m_grabbedObject)
        {
            //Debug.Log("PUT_DOWN," + m_grabbedObject.gameObject.name);
            m_grabbedObject.SetGrabbed(false);
            m_grabbedObject = null;
        }
    }

    void GrabObject(Interactable obj)
    {
        if (!obj.enabled) return;

        if (obj.isDragable)
        {
            m_grabbedObject = obj;
            //Debug.Log("PICK_UP," + obj.gameObject.name);
            obj.SetGrabbed(true);
        }
        else
        {
            m_clickedObject = obj;
            //Debug.Log("CLICK," + obj.gameObject.name);
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

    Transform CheckHit(Camera camera, int layerMask)
    {
        Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);
        if (debugRaycast)
        {
            Debug.DrawLine(cameraRay.origin, 1000.0f*cameraRay.direction, Color.red);
            //Debug.Log(cameraRay+" "+Input.mousePosition);
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, 1000.0f, layerMask))
        {
            if (debugRaycast) Debug.Log("HIT: " + hit.transform.gameObject.name);
            return hit.transform;
        }
        return null;
    }

    public Transform ComputeHitObject()
    {
        int layerMask = (1 << 8) | (1 << 9); // interactable + HUD
        Transform hit = CheckHit(Camera.main, layerMask);
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

    public void Print()
    {
        string contents = "---------------------------\n";
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
        contents += "---------------------------\n";
        Debug.Log(contents);
    }
}
