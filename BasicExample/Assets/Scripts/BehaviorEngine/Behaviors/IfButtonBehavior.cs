using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Runs the given behavior is the world state variable is true
class IfButtonBehavior : IfBehavior
{
    Button m_target = null;
    bool m_isClicked = false;

    public IfButtonBehavior(World w, string targetName, bool wait = false) : 
        base(w, null, wait)
    {
        targetName = targetName.Trim();
        if (!string.IsNullOrEmpty(targetName))
        {
            Transform targetX = w.Get(targetName);
            Debug.Assert(targetX != null);

            m_target = targetX.GetComponent<Button>();
            Debug.Assert(m_target != null);

            m_condition = this.CheckClick;
        }
    }

    public override void Setup()
    {
        base.Setup();
        m_isClicked = false;
        m_target.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
       m_isClicked = true;
       m_target.onClick.RemoveListener(OnClick);
    }

    bool CheckClick(World w)
    {
        return m_isClicked; 
    }
}
