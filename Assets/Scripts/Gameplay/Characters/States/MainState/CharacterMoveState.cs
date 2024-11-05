using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterMoveState : State
    {
        public override StateCategory Category { get; } = StateCategory.move;

        protected CharacterBaseMovementState currentMoveState;
        
        public GameObject GameObject  { get; set; }
        
        public float RotationSpeed { get; set; }
        
        public bool IsFacingTargetUsingDot(Transform objectTransform, Transform targetTransform, float thresholdDot = 1f)
        {
            // Направление на цель
            Vector3 directionToTarget = (targetTransform.position - objectTransform.position).normalized;

            // Значение Dot между forward направлением объекта и направлением на цель
            float dotToTarget = Vector3.Dot(objectTransform.forward, directionToTarget);

            // Если Dot больше порога, объект смотрит на цель
            return dotToTarget >= thresholdDot;
        }
        
        public override void Initialize()
        {
            currentMoveState?.Initialize();
        }
        
        public override void Enter()
        {
            DestermineState();
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }

        protected virtual void DestermineState()
        {
            
        }
        
        public void Rotate(GameObject target)
        {
            var targetTransform = target.transform;
            var objectTransform = GameObject.transform;
            
            var direction = targetTransform.position - objectTransform.position;
            if (direction == Vector3.zero) return;
            
            var currentTargetForRotate = Quaternion.LookRotation(direction, Vector3.up);
            GameObject.transform.rotation = Quaternion.RotateTowards(GameObject.transform.rotation, currentTargetForRotate, RotationSpeed * Time.deltaTime);
        }
    }

    public class CharacterMoveStateBuilder : StateBuilder<CharacterMoveState>
    {
        public CharacterMoveStateBuilder(CharacterMoveState instance) : base(instance)
        {
        }

        public CharacterMoveStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }

        public CharacterMoveStateBuilder SetRotationSpeed(float speed)
        {
            state.RotationSpeed = speed;
            return this;
        }
    }
}