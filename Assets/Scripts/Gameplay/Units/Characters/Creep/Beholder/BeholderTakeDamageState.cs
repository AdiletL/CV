using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderTakeDamageState : CreepTakeDamageState
    {
        private CreepSwitchAttackState creepSwitchAttackState;
        private CreepHealth creepHealth;
        
        public void SetCreepSwitchAttack(CreepSwitchAttackState creepSwitchAttackState) =>
            this.creepSwitchAttackState = creepSwitchAttackState;

        public override void Initialize()
        {
            base.Initialize();
            creepHealth = gameObject.GetComponent<CreepHealth>();
        }

        protected override void CountTimeAnimation()
        {
            countTimeAnimation += Time.deltaTime;
            if (countTimeAnimation > durationAnimation)
            {
                if (creepHealth.Damaging.TryGetComponent(out CharacterMainController character))
                {
                    creepSwitchAttackState.SetTarget(creepHealth.Damaging);
                    creepSwitchAttackState.ExitOtherStates();
                }
                else
                {
                    this.stateMachine.ExitCategory(Category, null);
                }
                countTimeAnimation = 0;
            }
        }
    }
    
    public class BeholderTakeDamageStateBuilder : CreepTakeDamageStateBuilder
    {
        public BeholderTakeDamageStateBuilder() : base(new BeholderTakeDamageState())
        {
        }
        
        public CreepTakeDamageStateBuilder SetCharacterSwitchAttack(CreepSwitchAttackState creepSwitchAttackState)
        {
            if (state is BeholderTakeDamageState beholderTakeDamageState)
                beholderTakeDamageState.SetCreepSwitchAttack(creepSwitchAttackState);

            return this;
        }
    }
}