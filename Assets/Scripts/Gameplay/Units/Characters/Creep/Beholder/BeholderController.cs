using Gameplay.Factory;
using Gameplay.Factory.Character.Creep;
using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderController : CreepController
    {
        [Space]
        [SerializeField] private Transform[] patrolPoints;
        
        [Space]
        [SerializeField] private SO_BeholderAttack so_BeholderAttack;
        [SerializeField] private SO_BeholderMove so_BeholderMove;

        private BeholderStateFactory beholderStateFactory;
        
        private CreepSwitchMoveState creepSwitchMoveState;
        private CreepSwitchAttackState creepSwitchAttackState;

        protected override CreepStateFactory CreateCreepStateFactory()
        {
            Vector3[] patrolPoints = new Vector3[this.patrolPoints.Length];
            for (int i = 0; i < this.patrolPoints.Length; i++)
                patrolPoints[i] = this.patrolPoints[i].position;
            
            return (BeholderStateFactory)new BeholderStateFactoryBuilder()
                .SetBeholderAttackConfig(so_BeholderAttack)
                .SetBeholderMoveConfig(so_BeholderMove)
                .SetCharacterAnimation(characterAnimation)
                .SetNavMeshAgent(navMeshAgent)
                .SetStateMachine(StateMachine)
                .SetPatrolPoints(patrolPoints)
                .SetUnitCenter(unitCenter)
                .SetGameObject(gameObject)
                .Build();
        }

        protected override CreepSwitchStateFactory CreateCreepSwitchStateFactory()
        {
            return (BeholderSwitchStateFactory)new BeholderSwitchStateFactoryBuilder()
                .SetBeholderMoveConfig(so_BeholderMove)
                .SetBeholderAttackConfig(so_BeholderAttack)
                .SetCreepStateFactory(creepStateFactory)
                .SetStateMachine(StateMachine)
                .SetNavMeshAgent(navMeshAgent)
                .SetCharacterAnimation(characterAnimation)
                .SetCharacterEndurance(characterEndurance)
                .SetUnitCenter(unitCenter)
                .SetGameObject(gameObject)
                .Build();
        }

        protected override UnitInformation CreateUnitInformation()
        {
            return new BeholderInformation(this);
        }

        public override int TotalDamage()
        {
            return creepSwitchAttackState.TotalDamage();
        }

        public override int TotalAttackSpeed()
        {
            return creepSwitchAttackState.TotalAttackSpeed();
        }

        public override float TotalAttackRange()
        {
            return creepSwitchAttackState.TotalAttackRange();
        }

        public override void Initialize()
        {
            base.Initialize();
            
            this.StateMachine.Initialize();
        }

        protected override void BeforeCreateStates()
        {
            base.BeforeCreateStates();
            beholderStateFactory = (BeholderStateFactory)creepStateFactory;
        }

        protected override void CreateStates()
        {
            var idleState = creepStateFactory.CreateState(typeof(BeholderIdleState));
            diContainer.Inject(idleState);
            this.StateMachine.AddStates(idleState);
        }

        protected override void CreateSwitchState()
        {
            creepSwitchMoveState = (BeholderSwitchMoveState)creepSwitchStateFactory.CreateSwitchMoveState(typeof(BeholderSwitchMoveState));
            diContainer.Inject(creepSwitchMoveState);

            creepSwitchAttackState = (BeholderSwitchAttackState)creepSwitchStateFactory.CreateSwitchAttackState(typeof(BeholderSwitchAttackState));
            diContainer.Inject(creepSwitchAttackState);
            
            creepSwitchMoveState.SetSwitchAttackState(creepSwitchAttackState);
            creepSwitchAttackState.SetSwitchMoveState(creepSwitchMoveState);
            
            beholderStateFactory.SetCreepSwitchMoveState(creepSwitchMoveState);
            beholderStateFactory.SetCreepSwitchAttackState(creepSwitchAttackState);
            
            creepSwitchMoveState.Initialize();
            creepSwitchAttackState.Initialize();
        }

        protected override void InitializeAllAnimations()
        {
            characterAnimation.AddClips(so_BeholderMove.IdleClip);
            characterAnimation.AddClips(so_BeholderMove.WalkClips);
            characterAnimation.AddClips(so_BeholderMove.RunClips);
            characterAnimation.AddClips(so_BeholderAttack.AttackClips);
            characterAnimation.AddClip(so_BeholderAttack.CooldownClip);
        }

        public override void Appear()
        {
            base.Appear();
            this.StateMachine.SetStates(desiredStates:typeof(BeholderIdleState));
        }
    }
}