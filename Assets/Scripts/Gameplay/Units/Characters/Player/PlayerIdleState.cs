using System;
using System.Collections;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;


namespace Unit.Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        private PlayerSwitchMove playerSwitchMove;

        private Vector3 targetPosition;

        public GameObject TargetForMove { get; set; }
        
        public override void Initialize()
        {
            base.Initialize();
            playerSwitchMove = (PlayerSwitchMove)CharacterSwitchMove;
        }
        

        public override void Subscribe()
        {
            base.Subscribe();
            this.StateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Update()
        {
            base.Update();
            CheckMove();
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            this.StateMachine.OnExitCategory -= OnExitCategory;
        }

        public override void Exit()
        {
            base.Exit();
            TargetForMove = null;
        }
        
        private void OnExitCategory(Machine.IState state)
        {
            if(!isActive) return;
            
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                PlayAnimation();
            }
        }
        
        public void SetTarget(GameObject target)
        {
            this.TargetForMove = target;
            playerSwitchMove.SetTarget(target);
        }
        
        private void CheckMove()
        {
            if(!TargetForMove) return;

            targetPosition = new Vector3(TargetForMove.transform.position.x, GameObject.transform.position.y, TargetForMove.transform.position.z);
            
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, targetPosition))
            {
                TargetForMove = null;
            }
            else
            {
                playerSwitchMove.ExitCategory(Category);
            }
        }
    }
    

    public class PlayerIdleStateBuilder : CharacterIdleStateBuilder
    {
        public PlayerIdleStateBuilder() : base(new PlayerIdleState())
        {
            
        }
    }
}