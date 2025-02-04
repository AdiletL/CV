using System;
using UnityEngine;

public abstract class Gravity : MonoBehaviour
{
    protected Vector3 velocity;
    protected bool isGravity = true;

    public float CurrentGravity { get; protected set; } = 1;
    
    public void ActivateGravity()
    {
        this.isGravity = true;
    }
    public void InActivateGravity()
    {
        this.isGravity = false;
    }

    public void SetVelocityY(float velocityY)
    {
        velocity.y = velocityY;
    }
    
    protected abstract void UseGravity();
}
