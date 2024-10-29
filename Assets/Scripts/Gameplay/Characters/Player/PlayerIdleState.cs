using System;
using System.Collections.Generic;
using Calculate;
using UnityEngine;


namespace Character
{
    public class PlayerIdleState : CharacterIdleState
    {
        public event Action OnFinishedMoveToEndTarget;

        public PathToPoint pathToPoint;
        public GameObject GameObject;

        private GameObject currentTarget;
        private GameObject finishTarget;
        
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
            if (finishTarget && !currentTarget) FindPathToPlatform();
            if (!currentTarget) return;

            if (GameObject.transform.position == currentTarget.transform.position)
            {
                currentCoordinates = FindPlatform.GetCoordinates(GameObject.transform.position);
                endTargetCoordinates = FindPlatform.GetCoordinates(finishTarget.transform.position);
                if (currentCoordinates == endTargetCoordinates)
                    OnFinishedMoveToEndTarget?.Invoke();

                FindPathToPlatform();
            }
            else
            {
                this.StateMachine.GetState<PlayerRunState>().SetTarget(currentTarget);
                this.StateMachine.SetState<PlayerRunState>();
                Debug.Log("TransitionRun");
            }
        }
        public override void Exit()
        {
            FindPlatform.GetPlatform(GameObject.transform.position + (Vector3.up * .5f), Vector3.down)?.RemoveGameObject(GameObject);
        }

        public void SetFinishTarget(GameObject finish)
        {
            this.finishTarget = finish;
            pathToPoint.SetTarget(finish.transform);
        }
        private void FindPathToPlatform()
        {
            if (pathToPlatformQueue.Count == 0)
            {
                pathToPlatformQueue = pathToPoint.FindPathToPoint();
            }
            else if (currentEndPosition != finishTarget.transform.position)
            {
                pathToPlatformQueue = pathToPoint.FindPathToPoint();
                currentEndPosition = finishTarget.transform.position;
            }
            currentTarget = pathToPlatformQueue.Count != 0 ? pathToPlatformQueue.Dequeue().gameObject : null;
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