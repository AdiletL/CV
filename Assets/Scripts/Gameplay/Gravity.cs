using System;
using UnityEngine;

public abstract class Gravity : MonoBehaviour
{
    protected Vector3 velocity;
    protected bool isGravity = true;

    public float CurrentGravity { get; protected set; } = 1;

    public abstract bool IsGrounded { get; }

    
    public void ActivateGravity()
    {
        this.isGravity = true;
    }
    public void InActivateGravity()
    {
        this.isGravity = false;
    }

    public virtual void AddVelocity(Vector3 velocity)
    {
        this.velocity += velocity;
    }
    
    protected abstract void UseGravity();
}
