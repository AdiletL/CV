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


        protected Queue<Platform> platformsQueue = new();
        
        public AnimationClip WalkClip { get; set; }
        public CreepAnimation CreepAnimation { get; set; }
        public Transform Center { get; set; }
        
        
        protected bool IsNear(Vector3 current, Vector3 target, float threshold = 0.01f)
        {
            return (current - target).sqrMagnitude <= threshold;
        }

        protected GameObject GetNextPoint()
        {
            if (StartPosition.HasValue && IsNear(GameObject.transform.position, StartPosition.Value))
            {
                pathFinding.SetStartPosition(GameObject.transform.position);
                pathFinding.SetTargetPosition(EndPlatform.transform.position);
            }
            else if (EndPosition.HasValue && IsNear(GameObject.transform.position, EndPosition.Value))
            {
                pathFinding.SetStartPosition(GameObject.transform.position);
                pathFinding.SetTargetPosition(StartPlatform.transform.position);
            }
            
            if(platformsQueue.Count == 0)
                platformsQueue = pathFinding.GetPath();

            if (platformsQueue.Count == 0)
                return null;
            
            return platformsQueue?.Peek().gameObject;
        }

        public override void Initialize()
        {
            base.Initialize();
            pathFinding = new PathToPointBuilder()
                .SetPosition(this.GameObject.transform.position, EndPlatform.transform.position)
                .Build();

        }

        public override void Enter()
        {
            base.Enter();
            currentTarget = GetNextPoint();
            
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
                    if (!Calculate.Move.IsFacingTargetUsingAngle(GameObject.transform.position, GameObject.transform.forward, currentTarget.transform.position))
                    {
                        Calculate.Move.Rotate(GameObject.transform, currentTarget.transform.position,
                            RotationSpeed, ignoreX: true, ignoreZ: true, ignoreY: false);
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
                currentTarget = GetNextPoint();
            }
        }

        public override void Exit()
        {
            base.Exit();
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