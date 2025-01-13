using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepGravity : Gravity
    {
        private CharacterController characterController;
        private Vector3 velocity;
        
        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            characterController.enabled = true;
            gravityForce = -8.5f;
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