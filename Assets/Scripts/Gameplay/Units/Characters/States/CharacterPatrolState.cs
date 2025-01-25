using UnityEngine;

namespace Unit.Character
{
    public class CharacterPatrolState : CharacterBaseMovementState, IPatrol
    {
        public ISwitchState CharacterSwitchAttack { get; set; }
        public Transform Start { get; set; }
        public Transform End { get; set; }
        public float RotationSpeed { get; set; }
        
        public Vector3? StartPosition { get; protected set; }
        public Vector3? EndPosition { get; protected set; }
        

        public override void Initialize()
        {
            StartPosition = Start?.position;
            EndPosition = End?.position;
        }

        public override void Enter()
        {
            base.Enter();
            StartPosition = Start?.position;
            EndPosition = End?.position;
        }

        public override void Update()
        {

        }
        
        public override void ExecuteMovement()
        {
            
        }
    }

    public class CharacterPatrolStateBuilder : CharacterBaseMovementStateBuilder
    {
        public CharacterPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }
        
        public CharacterPatrolStateBuilder SetStart(Transform start)
        {
            if (state is CharacterPatrolState patrolState)
            {
                patrolState.Start = start;
            }
            return this;
        }
        public CharacterPatrolStateBuilder SetEnd(Transform end)
        {
            if (state is CharacterPatrolState patrolState)
            {
                patrolState.End = end;
            }

            return this;
        }
        
        
        public CharacterPatrolStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if (state is CharacterPatrolState patrolState)
            {
                patrolState.RotationSpeed = rotationSpeed;
            }

            return this;
        }
        
        public CharacterPatrolStateBuilder SetCharacterSwitchAttack(ISwitchState characterSwitchAttack)
        {
            if (state is CharacterPatrolState patrolState)
            {
                patrolState.CharacterSwitchAttack = characterSwitchAttack;
            }

            return this;
        }
    }
}