using UnityEngine;

namespace Movement
{
    public class ForwardMovement : IMove
    {
        private GameObject gameObject;
        public float MovementSpeed { get; }

        public ForwardMovement(GameObject gameObject, float movementSpeed)
        {
            this.gameObject = gameObject;
            MovementSpeed = movementSpeed;
        }

        public void Initialize()
        {
            
        }
        
        public void Move()
        {
            gameObject.transform.Translate(Vector3.forward * (MovementSpeed * Time.deltaTime), Space.World);
        }
    }
}