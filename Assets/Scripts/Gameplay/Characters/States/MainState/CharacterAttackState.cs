using System;
using System.Collections.Generic;
using Machine;
using ScriptableObjects.Character;
using UnityEngine;
using IState = Unit.IState;

namespace Character
{
    public class CharacterAttackState : State
    {
        public override StateCategory Category { get; } = StateCategory.attack;
        

        protected IDamageble damageble;
        protected Dictionary<Type, CharacterBaseAttackState> attackStates = new();

        public GameObject GameObject { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public SO_CharacterAttack SO_CharacterAttack { get; set; }


        public override void Initialize()
        {
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
            //TODO: Switch type attack
        }

        public virtual void IncreaseStates(IState state)
        {
            
        }
    }

    public class CharacterAttackStateBuilder : StateBuilder<CharacterAttackState>
    {
        public CharacterAttackStateBuilder(CharacterAttackState instance) : base(instance)
        {
        }

        public CharacterAttackStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }

        public CharacterAttackStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.CharacterAnimation = characterAnimation;
            return this;
        }

        public CharacterAttackStateBuilder SetConfig(SO_CharacterAttack config)
        {
            state.SO_CharacterAttack = config;
            return this;
        }
    }
}