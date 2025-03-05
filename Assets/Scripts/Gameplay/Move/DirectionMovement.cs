using Unit;
using UnityEngine;

namespace Movement
{
    public class DirectionMovement : IMovement
    {
        private GameObject gameObject;
        private Vector3 direction;

        public Stat MovementSpeedStat { get; private set; } = new Stat();
        
        public DirectionMovement(GameObject gameObject, float movementSpeed)
        {
            this.gameObject = gameObject;
            this.MovementSpeedStat.AddValue(movementSpeed);
        }

        public void Initialize()
        {
            
        }
        
        public void SetDirection(Vector3 direction) => this.direction = direction.normalized;
        
        public void ExecuteMovement()
        {
            gameObject.transform.Translate(direction * (MovementSpeedStat.CurrentValue * Time.deltaTime), Space.World);
        }
    }
}