using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

namespace CTree
{
  /// <summary>
  /// Implements user interaction logic, such as selection and drag-and-drop, for scene assets.
  /// </summary>
  public class Interactable : MonoBehaviour
  {
    public static bool debugDraw = false;

    public delegate void ClickCb(Interactable i);

    public delegate void HoverCb(Interactable i);

    public delegate void DropCb(Interactable dragSource, GameObject dropTarget);

    public delegate void DragCb(Interactable dragSource, GameObject dropTarget);

    public delegate void PickupCb(Interactable dragSource);

    public enum Type { CLICKABLE, DRAGABLE, COPY_DRAGABLE};

    public Type interactionType = Type.CLICKABLE;

    private bool m_isDragging = false;
    private bool m_isClicked = false;
    private GameObject m_hoverObject = null;
    private GameObject m_dragObject = null; // temporary drag object
    private List<GameObject> m_dragTargets = new List<GameObject>(); // for dragable objects, the allowed target(s)

    private List<ClickCb> m_clickedCbs = new List<ClickCb>();
    private List<DropCb> m_dropCbs = new List<DropCb>();
    private List<PickupCb> m_pickupCbs = new List<PickupCb>();
    private List<DragCb> m_dragEnterCbs = new List<DragCb>();
    private List<DragCb> m_dragExitCbs = new List<DragCb>();
    private List<HoverCb> m_hoverCbs = new List<HoverCb>();
    private Bounds m_bounds = new Bounds();
    private Location m_location = null;
    private Vector3 m_offset;
    private Vector3 m_lastPointerPos;

    private void Start()
    {
      m_bounds = ComputeMaxBounds();
    }

    public void AddClickCb(ClickCb cb)
    {
      m_clickedCbs.Add(cb);
    }

    public void AddPickupCb(PickupCb cb)
    {
      m_pickupCbs.Add(cb);
    }

    public void AddDropCb(DropCb cb)
    {
      m_dropCbs.Add(cb);
    }

    public void AddHoverCb(HoverCb cb)
    {
      m_hoverCbs.Add(cb);
    }

    public void AddDragEnterCb(DragCb cb)
    {
      m_dragEnterCbs.Add(cb);
    }

    public void AddDragExitCb(DragCb cb)
    {
      m_dragExitCbs.Add(cb);
    }

    public void ClearCbs()
    {
      m_dropCbs.Clear();
      m_pickupCbs.Clear();
      m_dragEnterCbs.Clear();
      m_dragExitCbs.Clear();
      m_hoverCbs.Clear();
      m_dragTargets.Clear();

      // reset selection/grabbed state as well
      m_isDragging = false;
      m_isClicked = false;
      m_hoverObject = null; // temporary hover object
      m_dragObject = null; // temporary drag object
    }

    public bool isClickable()
    {
      return interactionType == Interactable.Type.CLICKABLE;
    }

    public bool isDragable()
    {
      return interactionType == Interactable.Type.DRAGABLE ||
          interactionType == Interactable.Type.COPY_DRAGABLE;
    }

    public bool CanDropOnto(GameObject target)
    {
      foreach (GameObject dragTarget in m_dragTargets)
      {
        if (target == dragTarget || target.transform.IsChildOf(dragTarget.transform))
        {
          Location loc = dragTarget.GetComponent<Location>();
          if (loc != null)
          {
              return loc.IsAvailable(this);
          }
          return true;
        } 
      }
      return false;
    }

    void Update ()
    {
      if (m_dragObject)
      {
        Vector3 screenMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_lastPointerPos.z);
        m_dragObject.transform.position = Camera.main.ScreenToWorldPoint(screenMousePos) + m_offset;
      }
    }

    public void AddDragTarget(GameObject target)
    {
      target.layer = gameObject.layer; // interactable
      foreach (Transform child in target.transform)
      {
        child.gameObject.layer = gameObject.layer;
      }

      m_dragTargets.Add(target);
    }

    bool CallbackActive(System.Delegate fn)
    {
      object o = fn.Target;
      if (o is Behavior)
      {
        Behavior b = o as Behavior;
        return b.IsActive();
      }

      return true;
    }

    public Location GetLocation()
    {
      return m_location;
    }

    public void RemoveLocation()
    {
       if (m_location) m_location.RemoveOcupant(this);
       m_location = null;
    }

    public void SetLocation(Location location)
    {
       m_location = location;
       if (location) location.AddOcupant(this);
    }

    public void OnDropTarget(GameObject target)
    {
      if (!enabled) return;

      Location location = target? target.GetComponent<Location>() : null;
      SetLocation(location);

      if (m_hoverObject != null) OnDragExit();
      foreach (DropCb cb in m_dropCbs) 
      {
        if (CallbackActive(cb))
        {
          cb(this, target);
        }
      }
    }

    public void OnDragEnter(GameObject target)
    {
      Debug.Log("ENTER "+m_hoverObject);
      foreach (DragCb cb in m_dragEnterCbs) 
      {
        if (CallbackActive(cb))
        {
          cb(this, target);
        }
      }
      m_hoverObject = target;
    }

    void OnDragExit()
    {
      Debug.Log("EXIT "+m_hoverObject);
      foreach (DragCb cb in m_dragExitCbs) 
      {
         if (CallbackActive(cb))
         {
            cb(this, m_hoverObject);
         }
      }
      m_hoverObject = null;
    }

    public void OnDragTarget(GameObject target)
    {
      if (!enabled) return;

      if (!m_hoverObject && target)
      {
        OnDragEnter(target);
      }
      else if (m_hoverObject && !target)
      {
        OnDragExit();
      }
    }

    public void OnHover()
    {
      if (!enabled) return;
      foreach (HoverCb cb in m_hoverCbs) 
      {
        if (CallbackActive(cb))
        {
          cb(this);
        }
      }
    }

    public void SetGrabbed(bool b)
    {
      if (!enabled) return;
      if (b != m_isDragging)
      {
        m_isDragging = b;
        if (m_isDragging && !m_dragObject)
        {
          RemoveLocation();

          Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
          Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);

          if (interactionType == Type.COPY_DRAGABLE)
          {
            m_dragObject = GameObject.Instantiate(gameObject, mousePos, Quaternion.identity);
            m_dragObject.transform.SetParent(transform.parent);
            m_dragObject.transform.localScale = transform.localScale;
          }
          else
          {
            m_dragObject = gameObject;
          }

          Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
          m_offset = m_dragObject.transform.position - mouseWorldPos;
          m_lastPointerPos = mousePos;

          Collider collider = m_dragObject.GetComponentInChildren<Collider>();
          collider.enabled = false;

          foreach (PickupCb cb in m_pickupCbs) 
          {
            if (CallbackActive(cb))
            {
              cb(this);
            }
          }
        }
        else if (!m_isDragging && m_dragObject)
        {
          if (interactionType == Type.COPY_DRAGABLE)
          {
            GameObject.Destroy(m_dragObject);
          }
          else
          {
            Collider collider = m_dragObject.GetComponentInChildren<Collider>();
            collider.enabled = true;
          }
          m_dragObject = null;
        }
      }
    }

    public bool GetGrabbed()
    {
      return m_isDragging;
    }

    public void SetClicked(bool b)
    {
      if (!enabled) return;
      m_isClicked = b;

      if (m_isClicked)
      {
        foreach (ClickCb cb in m_clickedCbs) 
        {
          if (CallbackActive(cb))
          {
            cb(this);
          }
        }
      }
    }

    public bool GetClicked()
    {
      // GetClicked is true only when the clicked status changes
      return m_isClicked;
    }

    public Bounds GetBounds()
    {
      return m_bounds;
    }

    Bounds ComputeMaxBounds()
    {
      Bounds b = new Bounds(transform.position, Vector3.zero);
      foreach (Renderer r in GetComponentsInChildren<Renderer>())
      {
        b.Encapsulate(r.bounds);
      }
      // Convert to relative coordinates
      //Debug.Log(gameObject.name + " MIN: " + b.min + " " + b.max);

      return b;
    }

    // FOR DEBUGGING
    static Material lineMaterial;
    static void CreateLineMaterial()
    {
      if (!lineMaterial)
      {
        // Unity has a built-in shader that is useful for drawing
        // simple colored things.
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
      }
    }

    void OnRenderObject()
    {
      if (debugDraw)
      {
         CreateLineMaterial();
         lineMaterial.SetPass(0);

         Vector3 min = m_bounds.min;
         Vector3 max = m_bounds.max;

         GL.PushMatrix();
         //GL.MultMatrix(gameObject.transform.localToWorldMatrix); 

         // draw top box
         GL.Begin(GL.LINES);
         GL.Color(new Color(1, 0, 0, 1));

         GL.Vertex3(min[0], min[1], min[2]);
         GL.Vertex3(min[0], min[1], max[2]);

         GL.Vertex3(min[0], min[1], min[2]);
         GL.Vertex3(max[0], min[1], min[2]);

         GL.Vertex3(max[0], min[1], min[2]);
         GL.Vertex3(max[0], min[1], max[2]);

         GL.Vertex3(min[0], min[1], max[2]);
         GL.Vertex3(max[0], min[1], max[2]);

         GL.End();
         GL.PopMatrix();
      }
    }
  }
}
