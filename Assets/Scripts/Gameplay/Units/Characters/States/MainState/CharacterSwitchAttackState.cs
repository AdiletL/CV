using System;
using System.Collections.Generic;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterSwitchAttackState : UnitSwitchAttackState
    {
        public override StateCategory Category { get; } = StateCategory.attack;
        
        protected SO_CharacterAttack so_CharacterAttack;

        protected IDamageable damageable;
        protected Collider[] findUnitColliders = new Collider[1];
        protected float rangeAttack;

        public CharacterEndurance CharacterEndurance { get; set; }
        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public LayerMask EnemyLayer { get; set; }


        public virtual bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange(Center.position, rangeAttack, EnemyLayer, ref findUnitColliders);
        }


        public override void Initialize()
        {
            so_CharacterAttack = (SO_CharacterAttack)this.SO_UnitAttack;
        }
        
        public override void Enter()
        {
        }

        public override void Update()
        {
            DestermineState();
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

    public class CharacterSwitchAttackStateBuilder : UnitSwitchAttackStateBuilder
    {
        public CharacterSwitchAttackStateBuilder(CharacterSwitchAttackState instance) : base(instance)
        {
        }

        public CharacterSwitchAttackStateBuilder SetGameObject(GameObject gameObject)
        {
            if(state is CharacterSwitchAttackState characterSwitchAttackState)
                characterSwitchAttackState.GameObject = gameObject;
            return this;
        }

        public CharacterSwitchAttackStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(state is CharacterSwitchAttackState characterSwitchAttackState)
                characterSwitchAttackState.CharacterAnimation = characterAnimation;
            return this;
        }
        
        public CharacterSwitchAttackStateBuilder SetCenter(Transform center)
        {
            if(state is CharacterSwitchAttackState characterSwitchAttackState)
                characterSwitchAttackState.Center = center;
            return this;
        }
        public CharacterSwitchAttackStateBuilder SetEnemyLayer(LayerMask layer)
        {
            if(state is CharacterSwitchAttackState characterSwitchAttackState)
                characterSwitchAttackState.EnemyLayer = layer;
            return this;
        }
        public CharacterSwitchAttackStateBuilder SetCharacterEndurance(CharacterEndurance characterEndurance)
        {
            if(state is CharacterSwitchAttackState characterSwitchAttackState)
                characterSwitchAttackState.CharacterEndurance = characterEndurance;
            return this;
        }
    }
}