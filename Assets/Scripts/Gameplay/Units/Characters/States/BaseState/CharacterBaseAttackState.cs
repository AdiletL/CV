﻿using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterBaseAttackState : UnitBaseAttackState
    {
        protected GameObject currentTarget;
        
        public override void Initialize()
        {
            
        }


        public override void Enter()
        {
            
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
        public override void Attack()
        {
            
        }

        public override void IncreaseStates(IState state)
        {
        }

        public override void ApplyDamage()
        {
        }
    }

    public class CharacterBaseAttackStateBuilder : UnitBaseAttackStateBuilder
    {
        public CharacterBaseAttackStateBuilder(CharacterBaseAttackState instance) : base(instance)
        {
            
        }
    }
}