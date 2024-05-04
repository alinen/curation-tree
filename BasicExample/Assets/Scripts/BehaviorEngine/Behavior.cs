using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public abstract class Behavior
{
    World m_world; 
    protected bool m_finished = false;
    protected bool m_isActive = false;
    
    protected int m_next = 0; // ID of next behavior - Depends on subclass
    public virtual int next
    {
        get { return m_next; }
        set { m_next = value; }
    }

    protected string m_name;
    public virtual string name
    {
        get {  return m_name; }
        set { m_name = value; }
    }

    public Behavior(World w) 
    {
        m_world = w;
        m_name = GetType().ToString();
    }

    public virtual void Tick() { }

    public virtual void Setup()
    {
        m_finished = false;
        m_isActive = true;
    }

    public virtual void TearDown()
    {
        m_isActive = false;
    }
    public virtual bool Finished()
    {
        return m_finished;
    }

    public bool IsActive()
    {
        return m_isActive;
    }

    protected World world
    {
        get { return m_world; }
    }

    public override string ToString()
    {
        return m_name;
    }

}
