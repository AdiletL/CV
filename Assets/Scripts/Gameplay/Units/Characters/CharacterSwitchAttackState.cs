using System;
using System.Collections.Generic;
using Gameplay.Equipment.Weapon;
using Gameplay.Factory.Character;
using Gameplay.Weapon;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterSwitchAttackState : CharacterSwitchState
    {
        protected GameObject currentTarget;
        protected SO_CharacterAttack so_CharacterAttack;
        
        protected Collider[] findUnitColliders = new Collider[1];

        protected CharacterSwitchMoveState characterSwitchMoveState;
        protected UnitEndurance unitEndurance;
        protected LayerMask enemyLayer;
        
        public float RangeAttack { get; protected set; }
        public float RangeAttackSqr { get; protected set; }
        
        
        public void SetConfig(SO_CharacterAttack config) => so_CharacterAttack = config;
        public void SetSwitchMoveState(CharacterSwitchMoveState characterSwitchMoveState) => this.characterSwitchMoveState = characterSwitchMoveState;
        public void SetUnitEndurance(UnitEndurance unitEndurance) => this.unitEndurance = unitEndurance;
        

        public virtual bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<IAttackable>(center.position, RangeAttack, enemyLayer, ref findUnitColliders);
        }

        public virtual int TotalDamage()
        {
            return so_CharacterAttack.Damage;
        }

        public virtual int TotalAttackSpeed()
        {
            return (int)so_CharacterAttack.AttackSpeed;
        }

        public virtual float TotalAttackRange()
        {
            return so_CharacterAttack.Range;
        }


        public override void Initialize()
        {
            enemyLayer = so_CharacterAttack.EnemyLayer;
        }

        public override void SetState()
        {
           
        }

        public override void ExitOtherStates()
        {
            
        }

        public override void ExitCategory(StateCategory category)
        {
            
        }

        public virtual void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
    }

    public class CharacterSwitchAttackStateBuilder : CharacterSwitchStateBuilder
    {
        public CharacterSwitchAttackStateBuilder(CharacterSwitchAttackState instance) : base(instance)
        {
        }

        public CharacterSwitchAttackStateBuilder SetConfig(SO_CharacterAttack config)
        {
            if(switchState is CharacterSwitchAttackState characterSwitchAttackState)
                characterSwitchAttackState.SetConfig(config);
            return this;
        }
        public CharacterSwitchAttackStateBuilder SetCharacterEndurance(UnitEndurance unitEndurance)
        {
            if(switchState is CharacterSwitchAttackState characterSwitchAttackState)
                characterSwitchAttackState.SetUnitEndurance(unitEndurance);
            return this;
        }
    }
}