using System;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerGravity : Gravity
    {
        private CharacterController characterController;
        private Vector3 velocity;
        
        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            characterController.enabled = true;
            gravityForce = -7;
        }

        protected override void UseGravity()
        {
            if (!isGravity)
            {
                velocity.y = 0;
                return;
            }
            
            velocity.y += gravityForce * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}