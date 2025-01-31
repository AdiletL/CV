using System.Collections.Generic;
using Calculate;
using Movement;
using Unit.Cell;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepPatrolState : CharacterPatrolState
    {
        protected PathFinding pathFinding;
        protected Rotation rotation;
        
        protected GameObject currentTarget;
        protected Vector3 direction;
        protected Vector3 currentTargetPosition;

        protected bool isCanMovement;
        
        protected Queue<CellController> pathToPoint = new();

        public AnimationClip WalkClip { get; set; }
        public CreepAnimation CreepAnimation { get; set; }
        public CharacterController CharacterController { get; set; }
        public Transform Center { get; set; }
        
        
        protected GameObject GetNextTarget()
        {
            if (StartPosition.HasValue && Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, StartPosition.Value))
            {
                pathFinding.SetStartPosition(gameObject.transform.position);
                pathFinding.SetTargetPosition(End.transform.position);
            }
            else if (EndPosition.HasValue && Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, EndPosition.Value))
            {
                pathFinding.SetStartPosition(gameObject.transform.position);
                pathFinding.SetTargetPosition(Start.transform.position);
            }
            
            if(pathToPoint.Count == 0)
                pathToPoint = pathFinding.GetPath();

            if (pathToPoint.Count == 0)
                return null;
            
            return pathToPoint?.Peek().gameObject;
        }

        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathFindingBuilder()
                .SetStartPosition(StartPosition.Value)
                .SetEndPosition(EndPosition.Value)
                .Build();
            rotation = new Rotation(gameObject.transform, RotationSpeed);
        }

        public override void Enter()
        {
            base.Enter();

            pathToPoint.Clear();
            FindNewPath();
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }
            
            CreepAnimation.ChangeAnimationWithDuration(WalkClip,  duration: 0.7f);
        }

        public override void Update()
        {
            base.Update();
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }

            RotateToTarget();
            ExecuteMovement();
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
        }

        private void FindNewPath()
        {
            if (pathToPoint.Count == 0)
            {
                pathFinding.SetStartPosition(gameObject.transform.position);
                
                var currentCell = Calculate.FindCell.GetCell(gameObject.transform.position, Vector3.down);
                var startCell = Calculate.FindCell.GetCell(StartPosition.Value, Vector3.down);
                var endCell = Calculate.FindCell.GetCell(EndPosition.Value, Vector3.down);
                
                if (currentCell == startCell) pathFinding.SetTargetPosition(EndPosition.Value);
                else if (currentCell == endCell) pathFinding.SetTargetPosition(StartPosition.Value);
                
                pathToPoint = pathFinding.GetPath();
            }
            
            if(pathToPoint.Count == 0) return;
            
            currentTarget = pathToPoint?.Peek()?.gameObject;
            rotation.SetTarget(currentTarget.transform);
        }

        public override void ExecuteMovement()
        {
            currentTargetPosition = new Vector3(currentTarget.transform.position.x, gameObject.transform.position.y, currentTarget.transform.position.z);
            
            if (Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, currentTarget.transform.position))
            {
                pathToPoint?.Dequeue();
                currentTarget = GetNextTarget();
                rotation.SetTarget(currentTarget.transform);
            }
            else
            {
                if(!isCanMovement) return;
                
                direction = (currentTargetPosition - gameObject.transform.position).normalized;
                CharacterController.Move(direction * (MovementSpeed * Time.deltaTime));
                
                //GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position,
                    //currentTarget.transform.position, MovementSpeed * Time.deltaTime);
            }
        }

        private void RotateToTarget()
        {
            if (Calculate.Move.IsFacingTargetUsingAngle(gameObject.transform.position, gameObject.transform.forward, currentTarget.transform.position))
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

    public class CreepPatrolStateBuilder : CharacterPatrolStateBuilder
    {
        public CreepPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }

        public CreepPatrolStateBuilder SetEnemyAnimation(CreepAnimation creepAnimation)
        {
            if (state is CreepPatrolState enemyPatrolState)
            {
                enemyPatrolState.CreepAnimation = creepAnimation;
            }

            return this;
        }

        public CreepPatrolStateBuilder SetWalkClip(AnimationClip walkClip)
        {
            if (state is CreepPatrolState enemyPatrolState)
            {
                enemyPatrolState.WalkClip = walkClip;
            }

            return this;
        }
        
        public CreepPatrolStateBuilder SetCenter(Transform center)
        {
            if (state is CreepPatrolState enemyPatrolState)
            {
                enemyPatrolState.Center = center;
            }

            return this;
        }
        
        public CreepPatrolStateBuilder SetCharacterController(CharacterController characterController)
        {
            if (state is CreepPatrolState enemyPatrolState)
            {
                enemyPatrolState.CharacterController = characterController;
            }

            return this;
        } 
    }
}