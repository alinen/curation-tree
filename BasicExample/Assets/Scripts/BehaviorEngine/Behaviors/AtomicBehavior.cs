
// Atomic behavior 
// Use this class to execute simple state changes as behavior
class AtomicBehavior : Behavior
{
    System.Action<World> m_fn = null;
    World m_w = null;

    public AtomicBehavior(World w, System.Action<World> fn) : base(w) 
    {
        m_fn = fn;
        m_w = w;
    }

    public override void Setup() 
    {
        base.Setup();
        m_fn(m_w);
        m_finished = true;
    }
}
