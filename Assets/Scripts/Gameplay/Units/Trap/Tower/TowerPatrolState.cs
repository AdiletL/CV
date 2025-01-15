using System.Collections.Generic;
using Calculate;
using Movement;
using Unit.Cell;
using UnityEngine;

namespace Unit.Trap.Tower
{
    public class TowerPatrolState : TrapMovementState, IPatrol
    {
        private MovementToPoint movementToPoint;
        private PathFinding pathFinding;
        private Rotation rotation;
        
        private GameObject currentTarget;
        private Queue<CellController> platformsQueue = new();

        private bool isCanMovement;
        
        public Transform Start { get; set; }
        public Transform End { get; set; }
        public Vector3? StartPosition { get; }
        public Vector3? EndPosition { get; }
        public float RotationSpeed { get; set; }
        

        private GameObject GetNextTarget()
        {
            if (StartPosition.HasValue && Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, StartPosition.Value))
            {
                pathFinding.SetStartPosition(GameObject.transform.position);
                pathFinding.SetTargetPosition(End.transform.position);
            }
            else if (EndPosition.HasValue && Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, EndPosition.Value))
            {
                pathFinding.SetStartPosition(GameObject.transform.position);
                pathFinding.SetTargetPosition(Start.transform.position);
            }
            
            if(platformsQueue.Count == 0)
                platformsQueue = pathFinding.GetPath();

            if (platformsQueue.Count == 0)
                return null;
            
            return platformsQueue?.Peek().gameObject;
        }
        
        public override void Initialize()
        {
            movementToPoint = new MovementToPoint(GameObject, MovementSpeed);
            pathFinding = new PathFindingBuilder()
                .SetStartPosition(StartPosition.Value)
                .SetEndPosition(EndPosition.Value)
                .Build();
            rotation = new Rotation(GameObject.transform, RotationSpeed);
            rotation.SetTarget(currentTarget.transform);
        }

        public override void Enter()
        {
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }
        }

        public override void Update()
        {
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }

            RotateToTarget();
            ExecuteMovement();
        }

        public override void LateUpdate()
        {
        }

        public override void Exit()
        {
            currentTarget = null;
        }

        public override void ExecuteMovement()
        {
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTarget.transform.position))
            {
                platformsQueue?.Dequeue();
                currentTarget = GetNextTarget();
            }
            else
            {
                if(!isCanMovement) return;
                
                movementToPoint?.ExecuteMovement();
            }
        }

        private void RotateToTarget()
        {
            if (Calculate.Move.IsFacingTargetUsingAngle(GameObject.transform.position, GameObject.transform.forward, currentTarget.transform.position))
            {
                isCanMovement = true;
            }
            else
            {
                rotation.Rotate();
                isCanMovement = false;
            }
        }
    }
    
    public class TowerPatrolStateBuilder : TrapMovementStateBuilder
    {
        public TowerPatrolStateBuilder(TrapMovementState instance) : base(instance)
        {
        }

        public TowerPatrolStateBuilder SetStart(Transform start)
        {
            if(state is TowerPatrolState towerPatrolState)
                towerPatrolState.Start = start;

            return this;
        }
        
        public TowerPatrolStateBuilder SetEnd(Transform end)
        {
            if(state is TowerPatrolState towerPatrolState)
                towerPatrolState.End = end;

            return this;
        }
        public TowerPatrolStateBuilder SetRotationSpeed(float speed)
        {
            if(state is TowerPatrolState towerPatrolState)
                towerPatrolState.RotationSpeed = speed;

            return this;
        }
    }
}