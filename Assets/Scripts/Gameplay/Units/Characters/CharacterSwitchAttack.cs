using System;
using System.Collections.Generic;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterSwitchAttack : UnitSwitchAttack, ISwitch
    {
        protected SO_CharacterAttack so_CharacterAttack;
        protected GameObject currentTarget;
        
        protected IDamageable damageable;
        protected Collider[] findUnitColliders = new Collider[1];

        
        public StateMachine StateMachine { get; set; }
        public CharacterEndurance CharacterEndurance { get; set; }
        public GameObject GameObject { get; set; }
        public Transform Center { get; set; }
        public CharacterAnimation CharacterAnimation { get; set; }
        public LayerMask EnemyLayer { get; set; }
        public float RangeAttack { get; protected set; }
        public float RangeAttackSqr { get; protected set; }
        public CharacterSwitchMove CharacterSwitchMove { get; protected set; }


        public virtual bool IsFindUnitInRange()
        {
            return Calculate.Attack.IsFindUnitInRange<IAttackable>(Center.position, RangeAttack, EnemyLayer, ref findUnitColliders);
        }
        

        public virtual void Initialize()
        {
            so_CharacterAttack = (SO_CharacterAttack)this.SO_UnitAttack;
        }

        public virtual void SetState()
        {
            
        }

        public virtual void ExitCategory(StateCategory category)
        {
            
        }

        public virtual void ExitOtherStates()
        {
            
        }

        public virtual void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
        
        public void SetSwitchMove(ISwitch moveSwitch)
        {
            CharacterSwitchMove = (CharacterSwitchMove)moveSwitch;
        }
    }

    public class CharacterSwitchAttackBuilder : UnitSwitchAttackStateBuilder<CharacterSwitchAttack>
    {
        public CharacterSwitchAttackBuilder(CharacterSwitchAttack instance) : base(instance)
        {
        }

        public CharacterSwitchAttackBuilder SetGameObject(GameObject gameObject)
        {
            if(state is CharacterSwitchAttack characterSwitchAttackState)
                characterSwitchAttackState.GameObject = gameObject;
            return this;
        }

        public CharacterSwitchAttackBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(state is CharacterSwitchAttack characterSwitchAttackState)
                characterSwitchAttackState.CharacterAnimation = characterAnimation;
            return this;
        }
        
        public CharacterSwitchAttackBuilder SetCenter(Transform center)
        {
            if(state is CharacterSwitchAttack characterSwitchAttackState)
                characterSwitchAttackState.Center = center;
            return this;
        }
        public CharacterSwitchAttackBuilder SetEnemyLayer(LayerMask layer)
        {
            if(state is CharacterSwitchAttack characterSwitchAttackState)
                characterSwitchAttackState.EnemyLayer = layer;
            return this;
        }
        public CharacterSwitchAttackBuilder SetCharacterEndurance(CharacterEndurance characterEndurance)
        {
            if(state is CharacterSwitchAttack characterSwitchAttackState)
                characterSwitchAttackState.CharacterEndurance = characterEndurance;
            return this;
        }

        public CharacterSwitchAttackBuilder SetStateMachine(StateMachine stateMachine)
        {
            if(state is CharacterSwitchAttack characterSwitchAttackState)
                characterSwitchAttackState.StateMachine = stateMachine;
            return this;
        }
    }
}