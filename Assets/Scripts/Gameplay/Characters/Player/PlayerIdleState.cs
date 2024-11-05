using System;
using System.Collections.Generic;
using Calculate;
using UnityEngine;


namespace Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        public event Action OnFinishedToTarget;

        private PathToPoint pathToPoint;
        private PlayerAttackState playerAttackState;
        private PlayerMoveState playerMoveState;
        
        private GameObject currentTargetForMove;
        private GameObject finishTargetForMove;
        
        private Vector3 currentEndPosition;
        private Vector2Int currentCoordinates, endTargetCoordinates;
        
        private Queue<Platform> pathToPlatformQueue = new();
        
        public GameObject GameObject { get; set; }
        public Transform EndPoint { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            pathToPoint = new PathToPointBuilder()
                .SetPosition(this.GameObject.transform, EndPoint)
                .Build();

            playerAttackState = this.StateMachine.GetState<PlayerAttackState>();
            playerMoveState = this.StateMachine.GetState<PlayerMoveState>();
            
            finishTargetForMove = EndPoint.gameObject;
        }

        public override void Enter()
        {
            base.Enter();
            FindPlatform.GetPlatform(GameObject.transform.position + (Vector3.up * .5f), Vector3.down)?.AddGameObject(GameObject);
        }

        public override void Update()
        {
            CheckAttack();
            CheckMove();
        }
        public override void Exit()
        {
            FindPlatform.GetPlatform(GameObject.transform.position + (Vector3.up * .5f), Vector3.down)?.RemoveGameObject(GameObject);
        }

        public void SetFinishTarget(GameObject finish)
        {
            this.finishTargetForMove = finish;
            pathToPoint.SetTarget(finish.transform);
            EndPoint = finish.transform;
        }

        private void CheckMove()
        {
            if (finishTargetForMove && !currentTargetForMove) FindPathToPlatform();
            if (!currentTargetForMove) return;

            if (GameObject.transform.position == currentTargetForMove.transform.position)
            {
                currentCoordinates = FindPlatform.GetCoordinates(GameObject.transform.position);
                endTargetCoordinates = FindPlatform.GetCoordinates(finishTargetForMove.transform.position);
                if (currentCoordinates == endTargetCoordinates)
                    OnFinishedToTarget?.Invoke();
                
                FindPathToPlatform();
            }
            else
            {
                this.StateMachine.ExitCategory(Category);
                playerMoveState.SetTarget(currentTargetForMove);
                this.StateMachine.SetStates(typeof(PlayerMoveState));
            }
        }

        private void CheckAttack()
        {
            var enemyGameObject = playerAttackState.CheckForwardEnemy();
            if (enemyGameObject?.transform.GetComponent<IHealth>() != null)
            {
                this.StateMachine.ExitOtherCategories(Category);
                this.StateMachine.SetStates(typeof(PlayerAttackState));
            }
        }
        
        private void FindPathToPlatform()
        {
            if (pathToPlatformQueue.Count == 0)
            {
                pathToPlatformQueue = pathToPoint.FindPathToPoint();
            }
            else if (currentEndPosition != finishTargetForMove.transform.position)
            {
                pathToPlatformQueue = pathToPoint.FindPathToPoint();
                currentEndPosition = finishTargetForMove.transform.position;
            }
            currentTargetForMove = pathToPlatformQueue.Count != 0 ? pathToPlatformQueue.Dequeue().gameObject : null;
        }
    }

    public class PlayerIdleStateBuilder : CharacterIdleStateBuilder
    {
        public PlayerIdleStateBuilder() : base(new PlayerIdleState())
        {
            
        }

        public PlayerIdleStateBuilder SetGameObject(GameObject gameObject)
        {
            if (state is PlayerIdleState playerIdleState)
            {
                playerIdleState.GameObject = gameObject;
            }
            return this;
        }
        
        public PlayerIdleStateBuilder SetEndPoint(Transform endPoint)
        {
            if (state is PlayerIdleState playerIdleState)
            {
                playerIdleState.EndPoint = endPoint;
            }

            return this;
        }
    }
}