using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class BeholderTakeDamageState : CreepTakeDamageState
    {
        private CreepHealth creepHealth;
        

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
                    stateMachine.GetState<CreepAttackState>().SetTarget(creepHealth.Damaging);
                    stateMachine.ExitOtherStates(typeof(CreepAttackState));
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
    }
}