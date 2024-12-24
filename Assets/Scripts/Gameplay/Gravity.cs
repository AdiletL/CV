using System;
using UnityEngine;

public abstract class Gravity : MonoBehaviour
{
    //private Vector3 origin;
    //private float originOffset = .5f;
    protected float gravityForce = .04f;
    //private float groundCheckDistance;
    
    //protected float gravity;
    protected bool isGravity = true;
    //protected bool isGrounded;

    public void ChangeGravity(bool isGravity)
    {
        this.isGravity = isGravity;
    }

    /*private void Start()
    {
        //groundCheckDistance = originOffset + .1f;
    }*/

    private void LateUpdate()
    {
        UseGravity();
    }

    /*private void LateUpdate()
    {
        if (!isGravity)
        {
            gravity = 0;
            return;
        }

        origin = new Vector3(transform.position.x, transform.position.y + originOffset, transform.position.z);
        //Debug.DrawRay(origin, (Vector3.down * groundCheckDistance), Color.black,  1);
        isGrounded = Physics.Raycast(origin,  Vector3.down, groundCheckDistance,  Layers.PLATFORM_LAYER);

        if (isGrounded)
        {
            gravity = 0f;
        }
        else
        {
            gravity -= gravityForce * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y + gravity, transform.position.z);
        }
    }*/
    protected abstract void UseGravity();
}
