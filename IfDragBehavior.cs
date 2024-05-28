using System.Collections.Generic;
using UnityEngine;

// Runs the given behavior is the world state variable is true
class IfDragBehavior : IfBehavior
{
    Interactable m_item = null;
    bool m_dragged = false;

    public IfDragBehavior(World w, string itemName) : 
        base(w, null)
    {
        itemName = itemName.Trim();
        if (!string.IsNullOrEmpty(itemName))
        {
            Transform itemX = w.Get(itemName);
            Debug.Assert(itemX != null);
            m_item = w.AddInteractable(itemX); 
            m_item.isDragable = true; 
            m_item.AddDragCb(DragCb);
            m_condition = this.CheckDrag;
        }
    }

    public override void Setup()
    {
        m_dragged = false;
        base.Setup();
    }

    void DragCb(Interactable source)
    {
        m_dragged = true;
    }

    bool CheckDrag(World w)
    {
        return m_dragged;
    }
}
