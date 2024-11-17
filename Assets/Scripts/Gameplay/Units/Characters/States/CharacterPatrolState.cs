using UnityEngine;

namespace Unit.Character
{
    public class CharacterPatrolState : CharacterBaseMovementState, IPatrol
    {
        public Platform StartPlatform { get; set; }
        public Platform EndPlatform { get; set; }
        
        public Vector3? StartPosition { get; set; }
        public Vector3? EndPosition { get; set; }
        

        public override void Initialize()
        {
            
        }

        public override void Enter()
        {
            StartPosition = StartPlatform?.transform.position;
            EndPosition = EndPlatform?.transform.position;
        }

        public override void Update()
        {

        }

        public override void Exit()
        {
        }

    }

    public class CharacterPatrolStateBuilder : CharacterBaseMovementStateBuilder
    {
        public CharacterPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }
        
        public CharacterPatrolStateBuilder SetStartPoint(Platform startPlatform)
        {
            if (state is CharacterPatrolState patrolState)
            {
                patrolState.StartPlatform = startPlatform;
            }
            return this;
        }
        public CharacterPatrolStateBuilder SetEndPoint(Platform endPlatform)
        {
            if (state is CharacterPatrolState patrolState)
            {
                patrolState.EndPlatform = endPlatform;
            }

            return this;
        }
    }
}