using UnityEngine;

namespace Movement
{
    public class MovementToPoint : IMove
    {
        private GameObject gameObject;
        private Vector3 point;
        
        public float MovementSpeed { get; }

        public MovementToPoint(GameObject gameObject, float movementSpeed)
        {
            this.gameObject = gameObject;
            this.MovementSpeed = movementSpeed;
        }

        public bool IsInPlace()
        {
            return Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, point);
        }
        
        public void SetPoint(Vector3 point)
        {
            this.point = point;
        }
        
        public void Move()
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, point, MovementSpeed * Time.deltaTime);
        }
    }
}