using System;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerGravity : Gravity
    {
        private CharacterController characterController;
        
        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            characterController.enabled = true;
            CurrentGravity = Physics.gravity.y + 2;
        }

        private void LateUpdate()
        {
            UseGravity();
        }

        protected override void UseGravity()
        {
            if (!isGravity)
            {
                velocity.y = 0;
                return;
            }
            
            velocity.y += CurrentGravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}