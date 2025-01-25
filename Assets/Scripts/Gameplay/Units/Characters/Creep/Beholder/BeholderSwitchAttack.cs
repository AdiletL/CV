using System;
using Gameplay.Damage;
using Machine;
using ScriptableObjects.Unit.Character.Creep;
using Zenject;

namespace Unit.Character.Creep
{
    public class BeholderSwitchAttack : CreepSwitchAttack
    {
        [Inject] private DiContainer diContainer;
        
        private SO_BeholderAttack so_BeholderAttack;
        private BeholderSwitchMove _beholderSwitchMove;
        private BeholderDefaultAttackState beholderDefaultAttackState;
        
        private BeholderDefaultAttackState CreateDefaultAttack()
        {
            return (BeholderDefaultAttackState)new BeholderDefaultAttackStateBuilder()
                .SetCenter(Center)
                .SetCharacterAnimation(CharacterAnimation)
                .SetCharacterSwitchMove(_beholderSwitchMove)
                .SetGameObject(GameObject)
                .SetAttackClips(so_BeholderAttack.AttackClips)
                .SetCooldownClip(so_BeholderAttack.CooldownClip)
                .SetRange(so_BeholderAttack.Range)
                .SetEnemyLayer(so_BeholderAttack.EnemyLayer)
                .SetAttackSpeed(so_BeholderAttack.AttackSpeed)
                .SetDamageable(damageable)
                .SetStateMachine(this.StateMachine)
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
            _beholderSwitchMove = (BeholderSwitchMove)CharacterSwitchMove;
            RangeAttack = so_BeholderAttack.Range;
            damageable = new NormalDamage(so_BeholderAttack.Damage, GameObject);
            diContainer.Inject(damageable);

            InitializeDefaultAttackState();
        }

        private void InitializeDefaultAttackState()
        {
            if (!this.StateMachine.IsStateNotNull(typeof(BeholderDefaultAttackState)))
            {
                beholderDefaultAttackState = CreateDefaultAttack();
                diContainer.Inject(beholderDefaultAttackState);
                beholderDefaultAttackState.Initialize();
                this.StateMachine.AddStates(beholderDefaultAttackState);
            }
        }

        public override void SetState()
        {
            base.SetState();
            
            InitializeDefaultAttackState();
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.StateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.StateMachine.SetStates(desiredStates: beholderDefaultAttackState.GetType());
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            
            InitializeDefaultAttackState();
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.StateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.StateMachine.ExitCategory(category, beholderDefaultAttackState.GetType());
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();

            InitializeDefaultAttackState();
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.StateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.StateMachine.ExitOtherStates(beholderDefaultAttackState.GetType());
        }
    }
    
    public class BeholderSwitchAttackBuilder : CreepSwitchSwitchAttackBuilder
    {
        public BeholderSwitchAttackBuilder() : base(new BeholderSwitchAttack())
        {
        }

    }
}