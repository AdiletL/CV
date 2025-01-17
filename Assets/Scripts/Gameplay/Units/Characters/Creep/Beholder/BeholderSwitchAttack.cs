using System;
using Gameplay.Damage;
using Machine;
using ScriptableObjects.Unit.Character.Creep;
using Zenject;

namespace Unit.Character.Creep
{
    public class BeholderSwitchAttack : CreepSwitchAttack
    {
        private DiContainer diContainer;
        
        private SO_BeholderAttack so_BeholderAttack;
        private BeholderSwitchMove beholderSwitchMove;
        private BeholderDefaultAttackState beholderDefaultAttackState;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }
        
        
        private BeholderDefaultAttackState CreateDefaultAttack()
        {
            return (BeholderDefaultAttackState)new BeholderDefaultAttackStateBuilder()
                .SetCenter(Center)
                .SetCharacterAnimation(CharacterAnimation)
                .SetCharacterSwitchMove(beholderSwitchMove)
                .SetGameObject(GameObject)
                .SetAttackClips(so_BeholderAttack.AttackClips)
                .SetCooldownClip(so_BeholderAttack.CooldownClip)
                .SetRange(so_BeholderAttack.Range)
                .SetEnemyLayer(so_BeholderAttack.EnemyLayer)
                .SetAmountAttackInSecond(so_BeholderAttack.AmountAttackInSecond)
                .SetDamageable(damageable)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();

            so_BeholderAttack = (SO_BeholderAttack)so_CharacterAttack;
            beholderSwitchMove = (BeholderSwitchMove)CharacterSwitchMove;
            RangeAttack = so_BeholderAttack.Range;
            damageable = new NormalDamage(so_BeholderAttack.Damage, GameObject);
            diContainer.Inject(damageable);
        }

        public override void SetState()
        {
            base.SetState();
            if (!this.StateMachine.IsStateNotNull(typeof(BeholderDefaultAttackState)))
            {
                beholderDefaultAttackState = CreateDefaultAttack();
                diContainer.Inject(beholderDefaultAttackState);
                beholderDefaultAttackState.Initialize();
                this.StateMachine.AddStates(beholderDefaultAttackState);
            }
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.StateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.StateMachine.SetStates(beholderDefaultAttackState.GetType());
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            if (!this.StateMachine.IsStateNotNull(typeof(BeholderDefaultAttackState)))
            {
                beholderDefaultAttackState = CreateDefaultAttack();
                diContainer.Inject(beholderDefaultAttackState);
                beholderDefaultAttackState.Initialize();
                this.StateMachine.AddStates(beholderDefaultAttackState);
            }
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.StateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.StateMachine.ExitCategory(category, beholderDefaultAttackState.GetType());
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();
            if (!this.StateMachine.IsStateNotNull(typeof(BeholderDefaultAttackState)))
            {
                beholderDefaultAttackState = CreateDefaultAttack();
                diContainer.Inject(beholderDefaultAttackState);
                beholderDefaultAttackState.Initialize();
                this.StateMachine.AddStates(beholderDefaultAttackState);
            }
            
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