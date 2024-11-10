using System.Collections.Generic;
using Calculate;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyPatrolState : CharacterPatrolState
    {
        protected virtual int checkEnemyLayer { get; }
        
        protected PathFinding pathFinding;

        protected GameObject currentTarget;

        protected EnemyAttackState enemyAttackState;
        protected EnemyMoveState enemyMoveState;

        protected Queue<Platform> platformsQueue = new();
        
        public AnimationClip WalkClip { get; set; }
        public EnemyAnimation EnemyAnimation { get; set; }

        protected Transform GetCurrentPoint()
        {
            if(!EndPlatform || !StartPlatform) return null;

            if (GameObject.transform.position == StartPosition)
                pathFinding.SetTarget(EndPlatform.transform);
            else if (GameObject.transform.position == EndPosition)
                pathFinding.SetTarget(StartPlatform.transform);
            
            if(platformsQueue.Count == 0)
                platformsQueue = pathFinding.GetPath();

            if (platformsQueue.Count == 0)
                return null;
            
            return platformsQueue?.Dequeue()?.transform;
        }

        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathToPointBuilder()
                .SetPosition(this.GameObject.transform, EndPlatform.transform)
                .Build();

            //enemyAttackState = this.StateMachine.GetState<EnemyAttackState>();
            enemyMoveState = this.StateMachine.GetState<EnemyMoveState>();
        }

        public override void Enter()
        {
            base.Enter();
            EnemyAnimation.ChangeAnimation(WalkClip, duration: 0.7f);
            var targetTransform = GetCurrentPoint();
            currentTarget = targetTransform ? targetTransform.gameObject : null;
        }

        public override void Update()
        {
            base.Update();
            
            if (!currentTarget)
                this.StateMachine.ExitCategory(Category);

            if (GameObject.transform.position != currentTarget.transform.position)
            {
                var enemyGameObject = Calculate.Attack.CheckForwardEnemy(this.GameObject, checkEnemyLayer);
                if (!enemyGameObject)
                {
                    Calculate.Move.Rotate(GameObject.transform, currentTarget.transform,
                        enemyMoveState.RotationSpeed);
                    
                    if (!Calculate.Move.IsFacingTargetUsingDot(GameObject.transform, currentTarget.transform))
                        return;
                    
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

    public class EnemyPatrolStateBuilder : CharacterPatrolStateBuilder
    {
        public EnemyPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }

        public EnemyPatrolStateBuilder SetEnemyAnimation(EnemyAnimation enemyAnimation)
        {
            if (state is EnemyPatrolState enemyPatrolState)
            {
                enemyPatrolState.EnemyAnimation = enemyAnimation;
            }

            return this;
        }

        public EnemyPatrolStateBuilder SetWalkClip(AnimationClip walkClip)
        {
            if (state is EnemyPatrolState enemyPatrolState)
            {
                enemyPatrolState.WalkClip = walkClip;
            }

            return this;
        }
    }
}