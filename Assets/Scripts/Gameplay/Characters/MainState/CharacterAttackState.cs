using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterAttackState : State
    {
        public StateMachine AttackStateMachine = new StateMachine();
        public GameObject GameObject;
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
        
        public GameObject CheckForwardEnemy()
        {
            if (Physics.Raycast(this.GameObject.transform.position + Vector3.up * .5f,
                    this.GameObject.transform.forward, out RaycastHit hit,
                    1.5f, Layers.ENEMY_LAYER))
            {
                return hit.transform.gameObject;
            }

            return null;
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
    }
}