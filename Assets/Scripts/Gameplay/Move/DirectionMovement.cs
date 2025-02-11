using UnityEngine;

namespace Movement
{
    public class DirectionMovement : IMovement
    {
        private GameObject gameObject;
        private Vector3 direction;
        public float MovementSpeed { get; }

        public DirectionMovement(GameObject gameObject, float movementSpeed)
        {
            this.gameObject = gameObject;
            this.MovementSpeed = movementSpeed;
        }

        public void Initialize()
        {
            
        }
        
        public void SetDirection(Vector3 direction) => this.direction = direction.normalized;
        
        public void ExecuteMovement()
        {
            gameObject.transform.Translate(direction * (MovementSpeed * Time.deltaTime), Space.World);
        }
    }
}