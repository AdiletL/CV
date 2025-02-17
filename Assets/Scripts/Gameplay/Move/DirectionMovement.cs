using UnityEngine;

namespace Movement
{
    public class DirectionMovement : IMovement
    {
        private GameObject gameObject;
        private Vector3 direction;
        public float BaseMovementSpeed { get; }
        public float CurrentMovementSpeed { get; }

        public DirectionMovement(GameObject gameObject, float movementSpeed)
        {
            this.gameObject = gameObject;
            this.CurrentMovementSpeed = movementSpeed;
        }

        public void Initialize()
        {
            
        }
        
        public void SetDirection(Vector3 direction) => this.direction = direction.normalized;
        
        public void ExecuteMovement()
        {
            gameObject.transform.Translate(direction * (CurrentMovementSpeed * Time.deltaTime), Space.World);
        }
    }
}