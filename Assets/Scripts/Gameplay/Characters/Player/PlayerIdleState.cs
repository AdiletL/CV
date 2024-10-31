using System;
using System.Collections.Generic;
using Calculate;
using UnityEngine;


namespace Character.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        public event Action OnFinishedMoveToEndTarget;

        public PathToPoint pathToPoint;
        public GameObject GameObject;

        private GameObject currentTargetForMove, currentTargetForAttack;
        private GameObject finishTargetForMove;
        
        private Vector3 currentEndPosition;
        private Vector2Int currentCoordinates, endTargetCoordinates;
        
        private Queue<Platform> pathToPlatformQueue = new();

        public override void Enter()
        {
            base.Enter();
            FindPlatform.GetPlatform(GameObject.transform.position + (Vector3.up * .5f), Vector3.down)?.AddGameObject(GameObject);
        }

        public override void Update()
        {
            CheckMove();
            CheckAttack();
        }
        public override void Exit()
        {
            FindPlatform.GetPlatform(GameObject.transform.position + (Vector3.up * .5f), Vector3.down)?.RemoveGameObject(GameObject);
        }

        public void SetFinishTarget(GameObject finish)
        {
            this.finishTargetForMove = finish;
            pathToPoint.SetTarget(finish.transform);
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
                    OnFinishedMoveToEndTarget?.Invoke();

                FindPathToPlatform();
            }
            else
            {
                this.StateMachine.GetState<PlayerMoveState>().SetTarget(currentTargetForMove);
                this.StateMachine.SetStates(typeof(PlayerMoveState));
            }
        }

        private void CheckAttack()
        {
            if (Physics.Raycast(GameObject.transform.position + Vector3.up * .5f, Vector3.forward, out RaycastHit hit,
                    1.5f, Layers.ENEMY_LAYER))
            {
                if (hit.transform.GetComponent<IHealth>() != null)
                {
                    currentTargetForAttack = hit.transform.gameObject;
                    this.StateMachine.GetState<PlayerAttackState>().SetTarget(currentTargetForAttack);
                    this.StateMachine.SetStates(typeof(PlayerAttackState));
                    Debug.Log(hit.transform.gameObject.name);
                }
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

        public PlayerIdleStateBuilder SetPathToPoint(PathToPoint pathToPoint)
        {
            if (state is PlayerIdleState playerIdleState)
            {
                playerIdleState.pathToPoint = pathToPoint;
            }
            return this;
        }

        public PlayerIdleStateBuilder SetGameObject(GameObject gameObject)
        {
            if (state is PlayerIdleState playerIdleState)
            {
                playerIdleState.GameObject = gameObject;
            }
            return this;
        }

    }
}