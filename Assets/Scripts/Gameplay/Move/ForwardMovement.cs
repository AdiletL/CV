using UnityEngine;

namespace Movement
{
    public class ForwardMovement : IMovement
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
        
        public void ExecuteMovement()
        {
            gameObject.transform.Translate(Vector3.forward * (MovementSpeed * Time.deltaTime), Space.World);
        }
    }
}