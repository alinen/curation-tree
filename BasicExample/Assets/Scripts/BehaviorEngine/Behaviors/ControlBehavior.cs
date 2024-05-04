using System.Collections.Generic;
using UnityEngine;

public abstract class ControlBehavior : Behavior
{
    protected List<Behavior> m_behaviors = new List<Behavior>();

    public int Count
    {
        get { return m_behaviors.Count; }
    }

    public ControlBehavior(World w) : base(w)
    {
    }

    public virtual void Add(Behavior b)
    {
        m_behaviors.Add(b);
    }

    public virtual void Insert(Behavior b, int index)
    {
        // inserts before index and shifts all elements back
        m_behaviors.Insert(index, b);
    }

    public virtual Behavior Get(int i)
    {
        return m_behaviors[i];
    }

    public virtual void Clear()
    {
        m_behaviors.Clear();
    }
}
