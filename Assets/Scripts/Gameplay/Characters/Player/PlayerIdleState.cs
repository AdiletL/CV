using System;
using System.Collections.Generic;
using Calculate;
using UnityEngine;


namespace Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        public event Action OnFinishedToTarget;
        public static event Action ASD;

        private PathFinding pathFinding;
        
        private GameObject currentTargetForMove;
        private GameObject finishTargetForMove;
        
        private Vector3 currentEndPosition;
        private Vector2Int currentCoordinates, endTargetCoordinates;
        
        private Queue<Platform> pathToPlatformQueue = new();
        
        public GameObject GameObject { get; set; }
        public Transform EndPoint { get; set; }

        private int asdf;

        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathToPointBuilder()
                .SetPosition(this.GameObject.transform, EndPoint)
                .Build();
            
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
            //pathToPlatformQueue.Clear();
        }

        public void SetFinishTarget(GameObject finish)
        {
            this.finishTargetForMove = finish;
            pathFinding.SetTarget(finish.transform);
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
                {
                    ASD?.Invoke();
                    OnFinishedToTarget?.Invoke();
                }
                
                FindPathToPlatform();
            }
            else
            {
                this.StateMachine.ExitCategory(Category);
                this.StateMachine.GetState<PlayerMoveState>().SetTarget(currentTargetForMove);
                this.StateMachine.SetStates(typeof(PlayerMoveState));
            }
        }

        private void CheckAttack()
        {
            var enemyGameObject = Calculate.Attack.CheckForwardEnemy(this.GameObject, Layers.ENEMY_LAYER);
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
                pathToPlatformQueue = pathFinding.GetPath();
            }
            else if (currentEndPosition != finishTargetForMove.transform.position)
            {
                pathToPlatformQueue = pathFinding.GetPath();
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