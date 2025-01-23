using Machine;

namespace Unit
{
    public abstract class UnitBaseAttackState : State, IAttack
    {
        public override StateCategory Category { get; } = StateCategory.attack;
        
        public IDamageable Damageable { get; set; }
        public int AttackSpeed { get; set; }

        public abstract void Attack();
        public abstract void ApplyDamage();
        public abstract void AddAttackSpeed(int amount);
        public abstract void RemoveAttackSpeed(int amount);
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

        public UnitBaseAttackStateBuilder SetAttackSpeed(int amount)
        {
            state.AttackSpeed = amount;
            return this;
        }
    }
}