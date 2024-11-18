using System;
using System.Collections.Generic;
using Calculate;
using UnityEngine;


namespace Unit.Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        public event Action OnFinishedToTarget;
        public static event Action ASD;
        
        private PathFinding pathFinding;
        private PlayerSwitchAttackState playerSwitchAttackState;

        private float checkEnemyCooldown = .3f;
        private float countCheckEnemyCooldown;
        
        private Queue<Platform> pathToPoint = new();
        
        private int asdf;
        
        public GameObject FinishTargetForMove { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathToPointBuilder()
                .SetPosition(this.GameObject.transform, FinishTargetForMove.transform)
                .Build();
            playerSwitchAttackState = this.StateMachine.GetState<PlayerSwitchAttackState>();
        }

        public override void Enter()
        {
            base.Enter();
            pathToPoint.Clear();
        }

        public override void Update()
        {
            CheckAttack();
            CheckMove();
        }


        public void SetFinishTarget(GameObject finish)
        {
            this.FinishTargetForMove = finish;
            pathFinding.SetTarget(finish.transform);
        }

        private void CheckMove()
        {
            if (GameObject.transform.position == FinishTargetForMove.transform.position)
            {
                pathToPoint.Clear();
                ASD?.Invoke();
                OnFinishedToTarget?.Invoke();
            }
            else
            {
                if (FinishTargetForMove && pathToPoint.Count == 0) FindPathToPoint();
                if (pathToPoint.Count == 0) return;


                this.StateMachine.ExitCategory(Category);
                this.StateMachine.GetState<PlayerSwitchMoveState>().SetPathToFinish(pathToPoint);
                this.StateMachine.GetState<PlayerSwitchMoveState>().SetFinish(FinishTargetForMove);
                this.StateMachine.SetStates(typeof(PlayerSwitchMoveState));
            }
        }
        private void FindPathToPoint()
        {
            if (pathToPoint.Count == 0)
            {
                pathToPoint = pathFinding.GetPath();
            }
        }
        
        private void CheckAttack()
        {
            countCheckEnemyCooldown += Time.deltaTime;
            if (countCheckEnemyCooldown > checkEnemyCooldown)
            {
                if (playerSwitchAttackState.IsCheckTarget())
                {
                    this.StateMachine.ExitOtherCategories(Category);
                    this.StateMachine.SetStates(typeof(PlayerSwitchAttackState));
                }

                countCheckEnemyCooldown = 0;
            }
        }
    }
    

    public class PlayerIdleStateBuilder : CharacterIdleStateBuilder
    {
        public PlayerIdleStateBuilder() : base(new PlayerIdleState())
        {
            
        }

        public PlayerIdleStateBuilder SetFinishTargetToMove(GameObject finish)
        {
            if (state is PlayerIdleState playerIdleState)
                playerIdleState.FinishTargetForMove = finish;

            return this;
        }
    }
}