namespace CTree
{
  /// <summary>
  /// Implements behaviors that perform actions that only require a single frame to execute.
  /// <remarks>
  /// Configure AtomicBehavior with a function argument that takes <c>World</c> as input. 
  /// For example, the following code enables the scene asset with name <c>objName</c>.
  /// <code>
  /// Behavior b = new AtomicBehavior(world, (w) => {
  ///    Transform xform = world.Get(objName.Trim());
  ///    xform.gameObject.SetActive(true);
  /// });
  /// </code>
  /// </remarks>
  /// </summary>

  public class AtomicBehavior : Behavior
  {
      System.Action<Behavior> m_fn = null;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="w">Object for accessing global state</param>
      /// <param name="fn">Function that performs the behavior</param>
      public AtomicBehavior(World w, System.Action<Behavior> fn) : base(w) 
      {
          m_fn = fn;
      }

      public override void Setup() 
      {
          base.Setup();
          m_fn(this);
          m_finished = true;
      }
  }
}
