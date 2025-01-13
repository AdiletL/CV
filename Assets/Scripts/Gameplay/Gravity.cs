using System;
using UnityEngine;

public abstract class Gravity : MonoBehaviour
{
    protected float gravityForce = .04f;
    
    protected bool isGravity = true;

    public void ActivateGravity()
    {
        this.isGravity = true;
    }
    public void InActivateGravity()
    {
        this.isGravity = false;
    }

    private void LateUpdate()
    {
        UseGravity();
    }
    
    protected abstract void UseGravity();
}
