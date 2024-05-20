using System.Collections.Generic;
using UnityEngine;

// Runs the given behavior is the world state variable is true
class IfHoverBehavior : IfBehavior
{
    Interactable m_target = null;
    
    public IfHoverBehavior(World w, string targetName, bool wait = false) : 
        base(w, null, wait)
    {
        targetName = targetName.Trim();
        if (!string.IsNullOrEmpty(targetName))
        {
            Transform targetX = w.Get(targetName);
            Debug.Assert(targetX != null);
            // ASN: Be careful not to overwrite properties from other behaviors
            m_target = w.AddInteractable(targetX); 
            m_condition = this.CheckClick;
        }
    }

    bool CheckClick(World w)
    {
        return m_target.GetClicked();
    }
}
