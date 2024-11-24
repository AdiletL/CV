using System.Collections.Generic;
using Calculate;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepPatrolState : CharacterPatrolState
    {
        protected virtual int checkEnemyLayer { get; }
        
        protected PathFinding pathFinding;

        protected GameObject currentTarget;

        protected CreepSwitchAttackState CreepSwitchAttackState;
        protected CreepSwitchMoveState CreepSwitchMoveState;

        protected Queue<Platform> platformsQueue = new();
        
        public AnimationClip WalkClip { get; set; }
        public CreepAnimation CreepAnimation { get; set; }
        public Transform Center { get; set; }
        
        
        protected bool IsNear(Vector3 current, Vector3 target, float threshold = 0f)
        {
            return (current - target).sqrMagnitude <= threshold;
        }

        protected Transform GetCurrentPoint()
        {
            if(!EndPlatform || !StartPlatform) return null;

            if (StartPosition.HasValue && IsNear(GameObject.transform.position, StartPosition.Value))
                pathFinding.SetTarget(EndPlatform.transform);
            else if (EndPosition.HasValue && IsNear(GameObject.transform.position, EndPosition.Value))
                pathFinding.SetTarget(StartPlatform.transform);
            
            if(platformsQueue.Count == 0)
                platformsQueue = pathFinding.GetPath();

            if (platformsQueue.Count == 0)
                return null;
            
            return platformsQueue?.Peek()?.transform;
        }

        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathToPointBuilder()
                .SetPosition(this.GameObject.transform, EndPlatform.transform)
                .Build();

            //enemyAttackState = this.StateMachine.GetState<EnemyAttackState>();
            CreepSwitchMoveState = this.StateMachine.GetState<CreepSwitchMoveState>();
        }

        public override void Enter()
        {
            base.Enter();
            var targetTransform = GetCurrentPoint();
            currentTarget = targetTransform ? targetTransform.gameObject : null;
            
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }
            
            CreepAnimation.ChangeAnimation(WalkClip,  duration: 0.7f);
        }

        public override void Update()
        {
            base.Update();
            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category);
                return;
            }

            if (!IsNear(GameObject.transform.position, currentTarget.transform.position))
            {
                var enemyGameObject = Calculate.Attack.CheckForwardEnemy(this.GameObject, Center.position, checkEnemyLayer);
                if (!enemyGameObject)
                {
                    if (!Calculate.Move.IsFacingTargetUsingAngle(GameObject.transform, currentTarget.transform))
                    {
                        Calculate.Move.Rotate(GameObject.transform, currentTarget.transform,
                            CreepSwitchMoveState.RotationSpeed);
                        return;
                    }
                    
                    GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position,
                        currentTarget.transform.position, MovementSpeed * Time.deltaTime);
                }
                else
                {
                    this.StateMachine.ExitCategory(Category);
                }
            }
            else
            {
                platformsQueue?.Dequeue();
                this.StateMachine.ExitCategory(Category);
            }
        }

        public override void Exit()
        {
            base.Exit();
            //platformsQueue.Clear();
            currentTarget = null;
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
    }
}