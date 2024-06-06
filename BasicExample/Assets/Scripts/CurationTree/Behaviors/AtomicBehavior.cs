namespace CTree
{
  // Atomic behavior 
  // Use this class to execute simple state changes as behavior
  class AtomicBehavior : Behavior
  {
      System.Action<World> m_fn = null;

      public AtomicBehavior(World w, System.Action<World> fn) : base(w) 
      {
          m_fn = fn;
      }

      public override void Setup() 
      {
          base.Setup();
          m_fn(world);
          m_finished = true;
      }
  }
}
