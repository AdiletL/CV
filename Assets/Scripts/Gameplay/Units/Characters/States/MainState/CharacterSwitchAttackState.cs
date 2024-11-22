using System;
using System.Collections.Generic;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterSwitchAttackState : State
    {
        public override StateCategory Category { get; } = StateCategory.attack;
        

        protected IDamageable Damageable;

        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public SO_CharacterAttack SO_CharacterAttack { get; set; }
        public int EnemyLayer { get; set; }


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
        public override void LateUpdate()
        {
            
        }
        public override void Exit()
        {
        }
        protected virtual void DestermineState()
        {
            //TODO: Switch type attack
        }

        public virtual void IncreaseStates(Unit.IState state)
        {
            
        }
    }

    public class CharacterSwitchAttackStateBuilder : StateBuilder<CharacterSwitchAttackState>
    {
        public CharacterSwitchAttackStateBuilder(CharacterSwitchAttackState instance) : base(instance)
        {
        }

        public CharacterSwitchAttackStateBuilder SetGameObject(GameObject gameObject)
        {
            state.GameObject = gameObject;
            return this;
        }

        public CharacterSwitchAttackStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.CharacterAnimation = characterAnimation;
            return this;
        }

        public CharacterSwitchAttackStateBuilder SetConfig(SO_CharacterAttack config)
        {
            state.SO_CharacterAttack = config;
            return this;
        }
        public CharacterSwitchAttackStateBuilder SetCenter(Transform center)
        {
            state.Center = center;
            return this;
        }
        public CharacterSwitchAttackStateBuilder SetEnemyLayer(int index)
        {
            state.EnemyLayer = index;
            return this;
        }
    }
}