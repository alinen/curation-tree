namespace CTree
{
  /// <summary>
  /// Runs the first behavior that has a satisfies a condition
  /// All sub-behaviors should be conditionals.
  /// </summary>
  public class SelectBehavior : ControlBehavior
  {
      IfBehavior m_selected = null;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="w">Object for accessing all global state.</param>
      public SelectBehavior(World w) : base(w)
      {
      }

      public override void TearDown()
      {
          base.TearDown();
          foreach (Behavior b in m_behaviors)
          {
              b.TearDown();
          }
      }

      public override void Setup()
      {
          m_selected = null;
          base.Setup();
          foreach (Behavior b in m_behaviors)
          {
              b.Setup();
          }
      }

      public override void Tick()
      {
          if (m_selected == null)
          {
              foreach (Behavior b in m_behaviors)
              {
                  IfBehavior ifb = b as IfBehavior;
                  if (m_selected == null && ifb != null && ifb.IsTrue())
                  {
                      m_selected = ifb;
                      break;
                  }
              }

          }

          if (m_selected != null)
          { 
              m_selected.Tick();
              m_finished = m_selected.Finished();
          }
      }
  }
}
