using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;
using Zenject;

namespace Unit.Character.Creep
{
    public class BeholderController : CreepController
    {
        [SerializeField] private Transform finalPath;
        [SerializeField] private SO_BeholderAttack so_BeholderAttack;
        [SerializeField] private SO_BeholderMove so_BeholderMove;

        private CreepSwitchMoveState creepSwitchMoveState;
        private CreepSwitchAttackState creepSwitchAttackState;

        
        private BeholderSwitchMoveState CreateBeholderSwitchMove()
        {
            return (BeholderSwitchMoveState)new BeholderSwitchSwitchMoveStateBuilder()
                .SetCharacterController(GetComponent<CharacterController>())
                .SetStart(Calculate.FindCell.GetCell(transform.position, Vector3.down).transform)
                .SetEnd(finalPath)
                .SetUnitAnimation(GetComponentInUnit<BeholderAnimation>())
                .SetGameObject(gameObject)
                .SetCenter(center)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        private BeholderIdleState CreateBeholderIdleState()
        {
            return (BeholderIdleState)new BeholderIdleStateBuilder()
                .SetCharacterAnimation(GetComponentInUnit<BeholderAnimation>())
                .SetIdleClips(so_BeholderMove.IdleClip)
                .SetCenter(center)
                .SetGameObject(gameObject)
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
                .SetCenter(center)
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
            
            this.StateMachine.SetStates(desiredStates:typeof(CreepIdleState));
        }

        protected override void CreateStates()
        {
            var idleState = CreateBeholderIdleState();
            diContainer.Inject(idleState);
            this.StateMachine.AddStates(idleState);
        }

        protected override void CreateSwitchState()
        {
            creepSwitchMoveState = CreateBeholderSwitchMove();
            diContainer.Inject(creepSwitchMoveState);

            creepSwitchAttackState = CreateBeholderSwitchAttackState();
            diContainer.Inject(creepSwitchAttackState);
            
            creepSwitchMoveState.SetSwitchAttackState(creepSwitchAttackState);
            creepSwitchAttackState.SetSwitchMoveState(creepSwitchMoveState);
            
            creepSwitchMoveState.Initialize();
            creepSwitchAttackState.Initialize();
        }

        protected override void InitializeAllAnimations()
        {
            //throw new System.NotImplementedException();
        }

        public override void Appear()
        {
            //this.StateMachine.SetStates(desiredStates:typeof(BeholderIdleState));
        }
    }
}