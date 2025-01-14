using Gameplay.Damage;
using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;
using Zenject;

namespace Unit.Character.Creep
{
    public class BeholderSwitchAttackState : CreepSwitchAttackState
    {
        private DiContainer diContainer;
        
        private SO_BeholderAttack so_BeholderAttack;


        [Inject]
        private void Construct(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }
        
        
        private BeholderDefaultAttackState CreateDefaultAttack()
        {
            return (BeholderDefaultAttackState)((UnitBaseAttackStateBuilder)new BeholderDefaultAttackStateBuilder()
                    .SetCenter(Center)
                    .SetCharacterAnimation(CharacterAnimation)
                    .SetGameObject(GameObject)
                    .SetAttackClips(so_BeholderAttack.AttackClips)
                    .SetCooldownClip(so_BeholderAttack.CooldownClip)
                    .SetRange(so_BeholderAttack.Range)
                    .SetAmountAttackInSecond(so_BeholderAttack.AmountAttackInSecond)
                    .SetEnemyLayer(so_BeholderAttack.EnemyLayer)).SetAmountAttackInSecond(so_BeholderAttack.AmountAttackInSecond)
                .SetDamageable(new NormalDamage(so_BeholderAttack.Damage, GameObject))
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();

            so_BeholderAttack = (SO_BeholderAttack)so_CharacterAttack;
            rangeAttack = so_BeholderAttack.Range;
        }


        protected override void DestermineState()
        {
            base.DestermineState();

            if (!this.StateMachine.IsStateNotNull(typeof(BeholderDefaultAttackState)))
            {
                var defaultAttack = CreateDefaultAttack();
                diContainer.Inject(defaultAttack);
                defaultAttack.Initialize();
                this.StateMachine.AddStates(defaultAttack);
            }
            
            this.StateMachine.SetStates(typeof(BeholderDefaultAttackState));
        }
    }
    
    public class BeholderSwitchAttackStateBuilder : CreepSwitchSwitchAttackStateBuilder
    {
        public BeholderSwitchAttackStateBuilder() : base(new BeholderSwitchAttackState())
        {
        }
    }
}