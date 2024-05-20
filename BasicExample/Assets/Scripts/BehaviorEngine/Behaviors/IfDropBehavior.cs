using System.Collections.Generic;
using UnityEngine;

// Runs the given behavior is the world state variable is true
class IfDropBehavior : IfBehavior
{
    Interactable m_item = null;
    Location m_target = null;
    bool m_dropped = false;

    public IfDropBehavior(World w, string itemName, string targetName) : 
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
            m_item.AddDropCb(DropCb);

            Transform targetX = w.Get(targetName);
            Debug.Assert(targetX != null);
            m_target = w.AddLocation(targetX); 
            m_item.AddDragTarget(targetX.gameObject);

            m_condition = this.CheckDrop;
        }
    }

    public override void Setup()
    {
        m_dropped = false;
        base.Setup();
    }

    void DropCb(Interactable source, GameObject target)
    {
        m_dropped = true;
    }

    bool CheckDrop(World w)
    {
        return m_dropped;
    }
}
