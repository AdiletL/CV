using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterAttackState : State
    {
        public StateMachine AttackStateMachine = new StateMachine();
        public IDamageble Damageble { get; set; }
        
        public override void Enter()
        {
            DestermineState();
        }

        public override void Update()
        {
            DestermineState();
            AttackStateMachine?.Update();
        }

        public override void Exit()
        {
            AttackStateMachine?.SetStates(typeof(CharacterAttackState));
        }
        protected virtual void DestermineState()
        {
            
        }
    }

    public class CharacterAttackStateBuilder : StateBuilder<CharacterAttackState>
    {
        public CharacterAttackStateBuilder(CharacterAttackState instance) : base(instance)
        {
        }
        
    }
}