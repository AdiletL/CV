﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterMoveState : State
    {
        public StateMachine MoveStateMachine  { get; set; } = new ();
        
        public GameObject GameObject  { get; set; }
        
        public override void Initialize()
        {
            MoveStateMachine?.Initialize();
        }
        
        public override void Enter()
        {
            DestermineState();
        }

        public override void Update()
        {
            DestermineState();
            MoveStateMachine?.Update();
        }

        public override void Exit()
        {
            MoveStateMachine.SetStates(typeof(CharacterMoveState));
        }

        protected virtual void DestermineState()
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
    }
}