using UnityEngine;
using UnityEngine.AI;

namespace Unit.Character.Creep
{
    public class CreepAgentPatrolState : CharacterPatrolState
    {
        protected NavMeshAgent navMeshAgent;
        protected CharacterAnimation characterAnimation;
        
        protected AnimationClip[] walkClips;
        
        protected int currentPointIndex;
        protected float durationAnimation;
        protected float rotationSpeed;
        protected const float STOPPING_DISTANCE = .2f;
        protected const string SPEED_MOVEMENT_NAME = "SpeedMovement";

        
        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetWalkClips(AnimationClip[] walkClips) => this.walkClips = walkClips;
        public void SetRotationSpeed(float rotationSpeed) => this.rotationSpeed = rotationSpeed;
        

        protected AnimationClip getRandomWalkClip() => walkClips[Random.Range(0, walkClips.Length)];
        

        public override void Initialize()
        {
            base.Initialize();
            navMeshAgent.speed = CurrentMovementSpeed;
            navMeshAgent.angularSpeed = rotationSpeed;
            navMeshAgent.stoppingDistance = STOPPING_DISTANCE;
        }

        public override void Enter()
        {
            base.Enter();
            if (navMeshAgent.isOnNavMesh)
                navMeshAgent.speed = CurrentMovementSpeed;
            SetTargetPoint();
            PlayAnimation();
        }

        public override void Update()
        {
            base.Update();
            ExecuteMovement();
        }

        public override void Exit()
        {
            base.Exit();
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
            }
        }

        private void SetTargetPoint()
        {
            if (!navMeshAgent.isOnNavMesh) return;
            navMeshAgent.destination = PatrolPoints[currentPointIndex];
            currentPointIndex = (currentPointIndex + 1) % PatrolPoints.Length;
            navMeshAgent.isStopped = false;
        }
        
        public override void ExecuteMovement()
        {
            base.ExecuteMovement();
            if (navMeshAgent.isOnNavMesh &&
                !navMeshAgent.pathPending && 
                navMeshAgent.remainingDistance < STOPPING_DISTANCE)
            {
                SetTargetPoint();
                PlayAnimation();
            }
        }
        
        protected void PlayAnimation()
        {
            UpdateDurationAnimation();
            characterAnimation.ChangeAnimationWithDuration(getRandomWalkClip(), duration: durationAnimation, SPEED_MOVEMENT_NAME);
        }

        protected void UpdateDurationAnimation()
        {
            durationAnimation = 1.5f / CurrentMovementSpeed;
            characterAnimation.SetSpeedClip(getRandomWalkClip(), duration: durationAnimation, SPEED_MOVEMENT_NAME);
        }
    }

    public class CreepAgentPatrolStateBuilder : CharacterPatrolStateBuilder
    {
        public CreepAgentPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }

        public CreepAgentPatrolStateBuilder SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if(state is CreepAgentPatrolState creepAgentPatrolState)
                creepAgentPatrolState.SetNavMeshAgent(navMeshAgent);
            return this;
        }
        
        public CreepAgentPatrolStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(state is CreepAgentPatrolState creepAgentPatrolState)
                creepAgentPatrolState.SetCharacterAnimation(characterAnimation);
            return this;
        }
        
        public CreepAgentPatrolStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if(state is CreepAgentPatrolState creepAgentPatrolState)
                creepAgentPatrolState.SetRotationSpeed(rotationSpeed);
            return this;
        }
        
        public CreepAgentPatrolStateBuilder SetWalkClips(AnimationClip[] walkClips)
        {
            if(state is CreepAgentPatrolState creepAgentPatrolState)
                creepAgentPatrolState.SetWalkClips(walkClips);
            return this;
        }
    }
}