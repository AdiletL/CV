using System;
using UnityEngine;

public abstract class Gravity : MonoBehaviour
{
    protected float gravityForce = .04f;
    
    protected bool isGravity = true;

    public void ChangeGravity(bool isGravity)
    {
        this.isGravity = isGravity;
    }

    private void LateUpdate()
    {
        UseGravity();
    }
    
    protected abstract void UseGravity();
}
