using Machine;
using UnityEngine;

namespace Unit
{
    public abstract class UnitBaseAttackState : State, IAttack
    {
        public override StateCategory Category { get; } = StateCategory.Attack;
        
        protected GameObject gameObject;
        protected Transform center;
        protected int damage;
        
        public IDamageable Damageable { get; protected set; }
        public int AttackSpeed { get; protected set; }
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;
        public void SetDamage(int damage) => this.damage = damage;
        public void SetAttackSpeed(int amount) => this.AttackSpeed = amount;
        

        public abstract IDamageable GetDamageable();
        

        public override void Initialize()
        {
            Damageable = GetDamageable();
        }

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

        public UnitBaseAttackStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }

        public UnitBaseAttackStateBuilder SetCenter(Transform center)
        {
            state.SetCenter(center);
            return this;
        }
        public UnitBaseAttackStateBuilder SetDamage(int damage)
        {
            state.SetDamage(damage);
            return this;
        }

        public UnitBaseAttackStateBuilder SetAttackSpeed(int amount)
        {
            state.SetAttackSpeed(amount);
            return this;
        }
    }
}