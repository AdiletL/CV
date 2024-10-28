using System;
using System.Collections.Generic;
using Calculate;
using Character;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerMove : CharacterMove
    {
        public event Action OnFinishedToTarget;
        
        private PlayerController playerController;

        private RunState runState;

        private Platform nextPlatform;

        private PathToPoint pathToPoint;

        private GameObject target;

        private Vector2Int currentCoordinates, targetCoordinates;

        private Queue<Platform> pathToPointQueue = new();

        public override void Initialize()
        {
            base.Initialize();

            playerController = (PlayerController)characterMainController;

            pathToPoint = new PathToPointBuilder()
                .SetPosition(transform, transform)
                .Build();

            var playerConfig = (SO_PlayerMove)config;

            runState = new RunStateBuild()
                .SetCharacters(this, characterMainController.components.GetComponentInGameObjects<CharacterAnimation>())
                .SetRunClip(playerConfig.RunClip)
                .SetRunSpeed(playerConfig.RunSpeed)
                .SetRotateSpeed(playerConfig.RotateSpeed)
                .SetCharacterTransform(transform)
                .Build();
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
            pathToPoint.SetTarget(target.transform);
        }

        private void Update()
        {
            if (!nextPlatform) return;

            if (transform.position == nextPlatform.transform.position)
            {
                ChangeState(idleState);

                currentCoordinates = FindPlatform.GetCoordinates(transform.position);
                targetCoordinates = FindPlatform.GetCoordinates(target.transform.position);
                if (currentCoordinates == targetCoordinates)
                    OnFinishedToTarget?.Invoke();

                FindNextStepPlatform();
            }
            else
            {
                runState.SetTarget(nextPlatform.transform);
                ChangeState(runState);
            }

            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            if (target && !nextPlatform) FindNextStepPlatform();
        }

        private void FindNextStepPlatform()
        {
            if (pathToPointQueue.Count == 0) pathToPointQueue = pathToPoint.FindPathToPoint();
            nextPlatform = pathToPointQueue.Count != 0 ? pathToPointQueue.Dequeue() : null;
        }

    }
}