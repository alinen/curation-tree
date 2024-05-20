using System.Collections.Generic;
using UnityEngine;

// Runs the given behavior is the world state variable is true
class IfMouseOverBehavior : IfBehavior
{
    Interactable m_item = null;
    bool m_over = false;

    public IfMouseOverBehavior(World w, string itemName) : 
        base(w, null)
    {
        itemName = itemName.Trim();
        if (!string.IsNullOrEmpty(itemName))
        {
            Transform itemX = w.Get(itemName);
            Debug.Assert(itemX != null);
            m_item = w.AddInteractable(itemX); 
            m_item.isDragable = true; 
            m_item.AddMouseOverCb(MouseCb);
            m_condition = this.CheckMouseOver;
        }
    }

    public override void Setup()
    {
        m_over = false;
        base.Setup();
    }

    void MouseCb(Interactable source)
    {
        m_over = true;
    }

    bool CheckMouseOver(World w)
    {
        return m_over;
    }
}
