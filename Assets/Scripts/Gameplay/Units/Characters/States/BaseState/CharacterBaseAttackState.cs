using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterBaseAttackState : State, IAttack
    {
        public override StateCategory Category { get; } = StateCategory.attack;
        
        protected GameObject currentTarget;
        
        public IDamageable Damageable { get; set; }
        public int AmountAttack { get; set; }

        
        public override void Initialize()
        {
            
        }
        
        public override void Enter()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void LateUpdate()
        {
            
        }

        public override void Exit()
        {
        }

        public virtual void Attack()
        {
            
        }

        public virtual void ApplyDamage()
        {
            
        }

        public virtual void IncreaseStates(Unit.IState state)
        {
            
        }
    }

    public class CharacterBaseAttackStateBuilder : StateBuilder<CharacterBaseAttackState>
    {
        public CharacterBaseAttackStateBuilder(CharacterBaseAttackState instance) : base(instance)
        {
            
        }

        public CharacterBaseAttackStateBuilder SetDamageble(IDamageable damageable)
        {
            state.Damageable = damageable;
            return this;
        }
    }
}