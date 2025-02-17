using UnityEngine;

namespace Movement
{
    public class MovementToPoint : IMovement
    {
        private GameObject gameObject;
        private Vector3 point;

        public float BaseMovementSpeed { get; }
        public float CurrentMovementSpeed { get; }

        public MovementToPoint(GameObject gameObject, float movementSpeed)
        {
            this.gameObject = gameObject;
            this.CurrentMovementSpeed = movementSpeed;
        }

        public bool IsInPlace()
        {
            return Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, point);
        }
        
        public void Initialize()
        {
            
        }
        
        public void SetPoint(Vector3 point)
        {
            this.point = point;
        }
        
        public void ExecuteMovement()
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, point, CurrentMovementSpeed * Time.deltaTime);
        }
    }
}