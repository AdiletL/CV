using Gameplay.Factory;
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
                .SetCharacterAnimation(characterAnimation)
                .SetNavMeshAgent(navMeshAgent)
                .SetStateMachine(StateMachine)
                .SetPatrolPoints(patrolPoints)
                .SetCreepMoveConfig(so_BeholderMove)
                .SetUnitCenter(unitCenter)
                .SetGameObject(gameObject)
                .Build();
        }

        private BeholderSwitchMoveState CreateBeholderSwitchMove()
        {
            return (BeholderSwitchMoveState)new BeholderSwitchSwitchMoveStateBuilder()
                .SetNavMeshAgent(navMeshAgent)
                .SetCreepStateFactory(beholderStateFactory)
                .SetUnitAnimation(GetComponentInUnit<BeholderAnimation>())
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        private BeholderSwitchAttackState CreateBeholderSwitchAttackState()
        {
            return (BeholderSwitchAttackState)new BeholderSwitchAttackStateAttackStateBuilder()
                .SetEnemyLayer(so_BeholderAttack.EnemyLayer)
                .SetConfig(so_BeholderAttack)
                .SetUnitAnimation(GetComponentInUnit<BeholderAnimation>())
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .SetCenter(unitCenter.Center)
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
            creepSwitchMoveState = CreateBeholderSwitchMove();
            diContainer.Inject(creepSwitchMoveState);

            //creepSwitchAttackState = CreateBeholderSwitchAttackState();
            //diContainer.Inject(creepSwitchAttackState);
            
            creepSwitchMoveState.SetSwitchAttackState(creepSwitchAttackState);
            //creepSwitchAttackState.SetSwitchMoveState(creepSwitchMoveState);
            
            creepSwitchMoveState.Initialize();
            beholderStateFactory.SetCreepSwitchMoveState(creepSwitchMoveState);
            //creepSwitchAttackState.Initialize();
        }

        protected override void InitializeAllAnimations()
        {
            characterAnimation.AddClips(so_BeholderMove.IdleClip);
            characterAnimation.AddClips(so_BeholderMove.WalkClips);
            characterAnimation.AddClips(so_BeholderMove.RunClips);
        }

        public override void Appear()
        {
            base.Appear();
            this.StateMachine.SetStates(desiredStates:typeof(BeholderIdleState));
        }
    }
}