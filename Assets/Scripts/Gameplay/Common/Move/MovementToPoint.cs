using Unit;
using UnityEngine;

namespace Gameplay.Movement
{
    public class MovementToPoint : IMovement
    {
        private GameObject gameObject;
        private Vector3 point;

        public Stat MovementSpeedStat { get; private set; } = new Stat();
        public bool IsCanMove { get; private set; }

        public MovementToPoint(GameObject gameObject, float movementSpeed)
        {
            this.gameObject = gameObject;
            this.MovementSpeedStat.AddCurrentValue(movementSpeed);
        }

        public bool IsInPlace()
        {
            return Calculate.Distance.IsDistanceToTargetSqr(gameObject.transform.position, point);
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
            if(IsCanMove)
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, point, MovementSpeedStat.CurrentValue * Time.deltaTime);
        }

        public void ActivateMovement()
        {
            IsCanMove = true;
        }

        public void DeactivateMovement()
        {
            IsCanMove = false;
        }
    }
}