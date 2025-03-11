using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Unit.Character.Creep
{
    public class CreepPatrolState : CharacterPatrolState
    {
        protected SO_CreepMove so_CreepMove;
        protected NavMeshAgent navMeshAgent;
        
        protected int currentPointIndex;
        protected const float STOPPING_DISTANCE = .2f;

        public Stat RotationSpeedStat { get; } = new Stat();

        public void SetNavMeshAgent(NavMeshAgent navMeshAgent) => this.navMeshAgent = navMeshAgent;
        

        public override void Initialize()
        {
            base.Initialize();
            so_CreepMove = (SO_CreepMove)so_CharacterMove;
            walkClips = so_CreepMove.WalkClips;
            characterAnimation.AddClips(walkClips);
            RotationSpeedStat.AddValue(so_CreepMove.RotateSpeed);
            MovementSpeedStat.AddValue(so_CreepMove.WalkSpeed);
            
            navMeshAgent.speed = MovementSpeedStat.CurrentValue;
            navMeshAgent.angularSpeed = RotationSpeedStat.CurrentValue;
            navMeshAgent.stoppingDistance = STOPPING_DISTANCE;
        }

        public override void Enter()
        {
            base.Enter();
            navMeshAgent.speed = MovementSpeedStat.CurrentValue;
            SetTargetPoint();
        }

        public override void Update()
        {
            base.Update();
            ExecuteMovement();
        }

        public override void Exit()
        {
            base.Exit();
            if(!navMeshAgent.isOnNavMesh) return;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        private void SetTargetPoint()
        {
            if(!navMeshAgent.isOnNavMesh) return;
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
                currentClip = getRandomClip(walkClips);
                PlayAnimation();
            }
        }
    }

    public class CreepAgentPatrolStateBuilder : CharacterPatrolStateBuilder
    {
        public CreepAgentPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }

        public CreepAgentPatrolStateBuilder SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if(state is CreepPatrolState creepAgentPatrolState)
                creepAgentPatrolState.SetNavMeshAgent(navMeshAgent);
            return this;
        }
    }
}