using UnityEngine;

namespace Gameplay.StateMachine
{
    public class MovementState : State, IMove
    {
        public GameObject GameObject;
        
        public float MovementSpeed { get; set; }
        public float RotationSpeed { get; set; }
        
        public override void Enter()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void Move()
        {
            throw new System.NotImplementedException();
        }

        public void Rotate(Vector3 direction)
        {
            throw new System.NotImplementedException();
        }
    }
}