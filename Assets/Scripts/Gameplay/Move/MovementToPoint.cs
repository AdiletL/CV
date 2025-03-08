using Unit;
using UnityEngine;

namespace Gameplay.Movement
{
    public class MovementToPoint : IMovement
    {
        private GameObject gameObject;
        private Vector3 point;

        public Stat MovementSpeedStat { get; private set; } = new Stat();
        
        public MovementToPoint(GameObject gameObject, float movementSpeed)
        {
            this.gameObject = gameObject;
            this.MovementSpeedStat.AddValue(movementSpeed);
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
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, point, MovementSpeedStat.CurrentValue * Time.deltaTime);
        }
    }
}