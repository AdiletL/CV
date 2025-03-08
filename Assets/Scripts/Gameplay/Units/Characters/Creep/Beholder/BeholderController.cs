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
        
        public BeholderStateFactory BeholderStateFactory { get; private set; }

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

        protected override UnitInformation CreateUnitInformation()
        {
            return new BeholderInformation(this);
        }
        
        protected override void BeforeCreateStates()
        {
            base.BeforeCreateStates();
            BeholderStateFactory = (BeholderStateFactory)CreepStateFactory;
        }

        protected override void CreateStates()
        {
            var idleState = BeholderStateFactory.CreateState(typeof(BeholderIdleState));
            diContainer.Inject(idleState);
            this.StateMachine.AddStates(idleState);
            
            var attackState = BeholderStateFactory.CreateState(typeof(BeholderAttackState));
            diContainer.Inject(attackState);
            this.StateMachine.AddStates(attackState);
            
            var moveState = BeholderStateFactory.CreateState(typeof(BeholderMoveState));
            diContainer.Inject(moveState);
            this.StateMachine.AddStates(moveState);
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

        public override void Disappear()
        {
            throw new System.NotImplementedException();
        }
    }
}