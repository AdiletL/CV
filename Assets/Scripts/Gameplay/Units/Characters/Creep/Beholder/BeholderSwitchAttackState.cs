using System;
using Gameplay.Damage;
using Machine;
using ScriptableObjects.Unit.Character.Creep;
using Zenject;

namespace Unit.Character.Creep
{
    public class BeholderSwitchAttackState : CreepSwitchAttackState
    {
        [Inject] private DiContainer diContainer;
        
        private SO_BeholderAttack so_BeholderAttack;
        private BeholderSwitchMoveState _beholderSwitchMoveState;
        private BeholderDefaultAttackState beholderDefaultAttackState;
        
        private BeholderDefaultAttackState CreateDefaultAttack()
        {
            return (BeholderDefaultAttackState)new BeholderDefaultAttackStateBuilder()
                .SetUnitAnimation(unitAnimation)
                .SetAttackClips(so_BeholderAttack.AttackClips)
                .SetCooldownClip(so_BeholderAttack.CooldownClip)
                .SetRange(so_BeholderAttack.Range)
                .SetEnemyLayer(so_BeholderAttack.EnemyLayer)
                .SetAttackSpeed(so_BeholderAttack.AttackSpeed)
                .SetDamage(so_BeholderAttack.Damage)
                .SetGameObject(gameObject)
                .SetCenter(center)
                .SetStateMachine(stateMachine)
                .Build();
        }

        public override int TotalDamage()
        {
            return beholderDefaultAttackState.Damageable.CurrentDamage +
                   beholderDefaultAttackState.Damageable.AdditionalDamage;
        }

        public override int TotalAttackSpeed()
        {
            return beholderDefaultAttackState.AttackSpeed;
        }

        public override float TotalAttackRange()
        {
            return beholderDefaultAttackState.Range;
        }

        public override void Initialize()
        {
            base.Initialize();

            so_BeholderAttack = (SO_BeholderAttack)so_CharacterAttack;
            RangeAttack = so_BeholderAttack.Range;

            InitializeDefaultAttackState();
        }

        private void InitializeDefaultAttackState()
        {
            if (!this.stateMachine.IsStateNotNull(typeof(BeholderDefaultAttackState)))
            {
                beholderDefaultAttackState = CreateDefaultAttack();
                diContainer.Inject(beholderDefaultAttackState);
                beholderDefaultAttackState.Initialize();
                this.stateMachine.AddStates(beholderDefaultAttackState);
            }
        }

        public override void SetState()
        {
            base.SetState();
            
            InitializeDefaultAttackState();
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.stateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.stateMachine.SetStates(desiredStates: beholderDefaultAttackState.GetType());
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            
            InitializeDefaultAttackState();
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.stateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.stateMachine.ExitCategory(category, beholderDefaultAttackState.GetType());
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();

            InitializeDefaultAttackState();
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.stateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.stateMachine.ExitOtherStates(beholderDefaultAttackState.GetType());
        }
    }
    
    public class BeholderSwitchAttackStateAttackStateBuilder : CreepSwitchAttackStateSwitchAttackStateBuilder
    {
        public BeholderSwitchAttackStateAttackStateBuilder() : base(new BeholderSwitchAttackState())
        {
        }

    }
}