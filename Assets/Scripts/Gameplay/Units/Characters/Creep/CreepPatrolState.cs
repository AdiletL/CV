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

        protected bool isCanMovement;
        
        protected Queue<CellController> cellQueue = new();
        
        public AnimationClip WalkClip { get; set; }
        public CreepAnimation CreepAnimation { get; set; }
        public Transform Center { get; set; }
        public LayerMask EnemyLayer { get; set; }
        
        
        protected GameObject GetNextTarget()
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
            
            if(cellQueue.Count == 0)
                cellQueue = pathFinding.GetPath();

            if (cellQueue.Count == 0)
                return null;
            
            return cellQueue?.Peek().gameObject;
        }

        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathFindingBuilder()
                .SetStartPosition(StartPosition.Value)
                .SetEndPosition(EndPosition.Value)
                .Build();
            rotation = new Rotation(GameObject.transform, RotationSpeed);
        }

        public override void Enter()
        {
            base.Enter();
            currentTarget = GetNextTarget();
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }
            
            rotation.SetTarget(currentTarget.transform);
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

            CheckAttack();
            RotateToTarget();
            Move();
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
        }

        public override void Move()
        {
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTarget.transform.position))
            {
                cellQueue?.Dequeue();
                currentTarget = GetNextTarget();
                rotation.SetTarget(currentTarget.transform);
            }
            else
            {
                if(!isCanMovement) return;
                
                GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position,
                    currentTarget.transform.position, MovementSpeed * Time.deltaTime);
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

        private void CheckAttack()
        {
            var enemyGameObject = Calculate.Attack.CheckForwardEnemy(this.GameObject, Center.position, EnemyLayer);
            if (enemyGameObject)
                this.StateMachine.ExitCategory(Category, null);
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
        
        public CreepPatrolStateBuilder SetEnemyLayer(LayerMask layer)
        {
            if (state is CreepPatrolState enemyPatrolState)
            {
                enemyPatrolState.EnemyLayer = layer;
            }

            return this;
        }
    }
}