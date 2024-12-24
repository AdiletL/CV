using Machine;

namespace Unit
{
    public abstract class UnitBaseAttackState : State, IAttack
    {
        public override StateCategory Category { get; } = StateCategory.attack;
        
        public IDamageable Damageable { get; set; }
        public int AmountAttack { get; set; }

        public abstract void Attack();
        public abstract void ApplyDamage();
        public abstract void IncreaseStates(IState state);
    }
    
    public abstract class UnitBaseAttackStateBuilder : StateBuilder<UnitBaseAttackState>
    {
        public UnitBaseAttackStateBuilder(UnitBaseAttackState instance) : base(instance)
        {
        }
        
        public UnitBaseAttackStateBuilder SetDamageable(IDamageable damageable)
        {
            state.Damageable = damageable;
            return this;
        }

        public UnitBaseAttackStateBuilder SetAmountAttack(int amount)
        {
            state.AmountAttack = amount;
            return this;
        }
    }
}