﻿using UnityEngine;

namespace Unit.Character
{
    public class CharacterPatrolState : CharacterBaseMovementState, IPatrol
    {
        public Vector3[] PatrolPoints { get; protected set; }
        
        public void SetPatrolPoints(Vector3[] points) => PatrolPoints = points;
        
        
        public override void ExecuteMovement()
        {
            
        }

    }

    public class CharacterPatrolStateBuilder : CharacterBaseMovementStateBuilder
    {
        public CharacterPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }
        
        public CharacterPatrolStateBuilder SetPatrolPoints(Vector3[] points)
        {
            if (state is CharacterPatrolState patrolState)
                patrolState.SetPatrolPoints(points);
            return this;
        }
    }
}