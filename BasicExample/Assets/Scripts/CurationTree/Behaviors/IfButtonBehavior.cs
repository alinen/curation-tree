using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CTree
{
  /// <summary>
  /// Implements a Behavior that executes all sub-behaviors if a UI button is clicked.
  /// This behavior is based on callabcks on Unity Button components.
  /// </summary>
  public class IfButtonBehavior : IfBehavior
  {
      Button m_target = null;
      bool m_isClicked = false;

      /// <summary>
      /// Constructor 
      /// </summary>
      /// <param name="w">Object for accessing global state.</param>
      /// <param name="targetName">Name of an asset that contains a Button component</param>
      public IfButtonBehavior(World w, string targetName) : base(w, null)
      {
          targetName = targetName.Trim();
          if (!string.IsNullOrEmpty(targetName))
          {
              Transform targetX = w.Get(targetName);
              Debug.Assert(targetX != null);

              m_target = targetX.GetComponent<Button>();
              Debug.Assert(m_target != null);
              m_target.onClick.AddListener(OnClick);

              m_condition = this.CheckClick;
          }
      }

      public override void Setup()
      {
          base.Setup();
      }

      void OnClick()
      {
         m_isClicked = true;
      }

      bool CheckClick(World w)
      {
          return m_isClicked; 
      }
  }
}
