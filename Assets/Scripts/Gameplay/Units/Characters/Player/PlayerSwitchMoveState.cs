using System;
using System.Collections.Generic;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSwitchMoveState : CharacterSwitchMoveState
    {
        private SO_PlayerMove so_PlayerMove;
        private GameObject finish;

        private Queue<Platform> pathToFinish = new();
        
        public Transform Center { get; set; }


        private PlayerRunState CreateRunState()
        {
            return (PlayerRunState)new PlayerRunStateBuilder()
                .SetMoveConfig(so_PlayerMove)
                .SetCenter(Center)
                .SetCharacterAnimation(this.CharacterAnimation)
                .SetRunClips(so_PlayerMove.RunClip)
                .SetGameObject(GameObject)
                .SetMovementSpeed(so_PlayerMove.RunSpeed)
                .SetStateMachine(this.StateMachine)
                .Build();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            so_PlayerMove = (SO_PlayerMove)SO_CharacterMove;
        }
        
        protected override void DestermineState()
        {
            //TODO: Select type move

            if (!this.StateMachine.IsStateNotNull(typeof(PlayerRunState)))
            {
                var newState = CreateRunState();
                newState.Initialize();
                this.StateMachine.AddStates(newState);
            }

            var runState = this.StateMachine.GetState<PlayerRunState>();
            runState.SetPathToFinish(pathToFinish);
            runState.SetFinish(finish);
            
            this.StateMachine.SetStates(typeof(PlayerRunState));
        }
        
        public void SetFinish(GameObject target)
        {
            finish = target;
        }
        public void SetPathToFinish(Queue<Platform> path)
        {
            this.pathToFinish = path;
        }

    }

    public class PlayerMoveStateBuilder : CharacterMoveStateBuilder
    {
        public PlayerMoveStateBuilder() : base(new PlayerSwitchMoveState())
        {
        }
        
        public PlayerMoveStateBuilder SetCenter(Transform center)
        {
            if(state is PlayerSwitchMoveState playerSwitchMoveState)
                playerSwitchMoveState.Center = center;

            return this;
        }
    }
}