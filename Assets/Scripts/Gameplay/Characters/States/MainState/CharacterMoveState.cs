using System;
using System.Collections.Generic;
using Machine;
using ScriptableObjects.Character;
using UnityEngine;
using IState = Character.IState;

namespace Character
{
    public class CharacterMoveState : State
    {
        public override StateCategory Category { get; } = StateCategory.move;
        public SO_CharacterMove SO_CharacterMove { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public GameObject GameObject  { get; set; }
        public float RotationSpeed { get; set; }

        protected Dictionary<Type, CharacterBaseMovementState> movementStates = new();
        
        public override void Initialize()
        {
            RotationSpeed = this.SO_CharacterMove.RotateSpeed;
        }
        
        public override void Enter()
        {
        }

        public override void Update()
        {
            DestermineState();
        }

        public override void Exit()
        {
        }

        protected virtual void DestermineState()
        {
            
        }

        public virtual void IncreaseStates(IState state)
        {
            
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

        public CharacterMoveStateBuilder SetConfig(SO_CharacterMove config)
        {
            state.SO_CharacterMove = config;
            return this;
        }

        public CharacterMoveStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.CharacterAnimation = characterAnimation;
            return this;
        }
    }
}