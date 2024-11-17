using System;
using System.Collections.Generic;
using Calculate;
using UnityEngine;


namespace Unit.Character.Player
{
    public class PlayerIdleIdleState : CharacterIdleIdleState
    {
        public event Action OnFinishedToTarget;
        public static event Action ASD;
        
        private PathFinding pathFinding;
        
        private Queue<Platform> pathToPoint = new();
        
        private int asdf;
        
        public GameObject FinishTargetForMove { get; set; }
        public float RangeAttack { get; set; }
        

        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathToPointBuilder()
                .SetPosition(this.GameObject.transform, FinishTargetForMove.transform)
                .Build();
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

        private void CheckAttack()
        {
            var enemyGameObject = Calculate.Attack.CheckForwardEnemy(this.GameObject, Layers.ENEMY_LAYER, RangeAttack);
            if (enemyGameObject?.transform.GetComponent<IHealth>() != null)
            {
                this.StateMachine.ExitOtherCategories(Category);
                this.StateMachine.SetStates(typeof(PlayerSwitchAttackState));
            }
        }
        
        private void FindPathToPoint()
        {
            if (pathToPoint.Count == 0)
            {
                pathToPoint = pathFinding.GetPath();
            }
        }
    }
    

    public class PlayerIdleStateBuilder : CharacterIdleStateBuilder
    {
        public PlayerIdleStateBuilder() : base(new PlayerIdleIdleState())
        {
            
        }

        public PlayerIdleStateBuilder SetFinishTargetToMove(GameObject finish)
        {
            if (state is PlayerIdleIdleState playerIdleState)
                playerIdleState.FinishTargetForMove = finish;

            return this;
        }

        public PlayerIdleStateBuilder SetRangeAttack(float range)
        {
            if(state is PlayerIdleIdleState playerIdleState)
                playerIdleState.RangeAttack = range;
            
            return this;
        }
    }
}