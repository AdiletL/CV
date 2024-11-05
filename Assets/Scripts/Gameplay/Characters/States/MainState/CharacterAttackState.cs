using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterAttackState : State
    {
        public override StateCategory Category { get; } = StateCategory.attack;
        
        protected CharacterBaseAttackState curretAttackState;

        private Vector3 originRayForward;
        private RaycastHit[] enemyHits = new RaycastHit[1];

        public GameObject GameObject { get; set; }
        
        
        public override void Initialize()
        {
            curretAttackState?.Initialize();
            originRayForward = Vector3.up * .5f;
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
        
        public GameObject CheckForwardEnemy()
        {
            var hitCount = Physics.RaycastNonAlloc(this.GameObject.transform.position + originRayForward,
                this.GameObject.transform.forward, enemyHits,
                .5f, Layers.ENEMY_LAYER);

            // Если был хотя бы один результат
            if (hitCount > 0)
            {
                RaycastHit hit = enemyHits[0];
    
                // Проверяем, что найденный объект не совпадает с текущим GameObject
                if (hit.transform.gameObject != this.GameObject)
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