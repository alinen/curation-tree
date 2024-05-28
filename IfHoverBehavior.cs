using System.Collections.Generic;
using UnityEngine;

class IfHoverBehavior : IfBehavior
{
    Interactable m_item = null;
    Location m_target = null;
    bool m_hover = false;

    public IfHoverBehavior(World w, string itemName, string targetName) : 
        base(w, null)
    {
        itemName = itemName.Trim();
        targetName = targetName.Trim();
        if (!string.IsNullOrEmpty(itemName) && !string.IsNullOrEmpty(targetName))
        {
            Transform itemX = w.Get(itemName);
            Debug.Assert(itemX != null);
            m_item = w.AddInteractable(itemX); 
            m_item.isDragable = true; 
            m_item.AddHoverCb(HoverCb);

            Transform targetX = w.Get(targetName);
            Debug.Assert(targetX != null);
            m_target = w.AddLocation(targetX); 
            m_item.AddDragTarget(targetX.gameObject);

            m_condition = this.CheckHover;
        }
    }

    public override void Setup()
    {
        m_hover = false;
        base.Setup();
    }

    void HoverCb(Interactable source, GameObject target)
    {
        m_hover = (target == m_target.gameObject);
    }

    bool CheckHover(World w)
    {
        return m_hover;
    }
}
