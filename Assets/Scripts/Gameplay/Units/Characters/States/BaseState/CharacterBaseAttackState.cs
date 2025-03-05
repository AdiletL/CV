using Gameplay.Damage;
using UnityEngine;
using Zenject;

namespace Unit.Character
{
    public class CharacterBaseAttackState : UnitBaseAttackState
    {
        [Inject] protected DiContainer diContainer;
        
        protected GameObject currentTarget;
        public Stat DamageStat { get; protected set; } = new ();
        
        public override IDamageable GetDamageable()
        {
            var damageable = new NormalDamage(gameObject, DamageStat);
            diContainer.Inject(damageable);
            return damageable;
        }
        
        public override void Update()
        {
            
        }

        public override void LateUpdate()
        {
            
        }


        public override void Attack()
        {
            
        }
        
        public override void ApplyDamage()
        {
        }

        public virtual void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
        
        public override void AddAttackSpeed(int amount)
        {
            AttackSpeed += amount;
        }

        public override void RemoveAttackSpeed(int amount)
        {
            AttackSpeed -= amount;
        }
    }

    public class CharacterBaseAttackStateBuilder : UnitBaseAttackStateBuilder
    {
        public CharacterBaseAttackStateBuilder(CharacterBaseAttackState instance) : base(instance)
        {
            
        }
        
    }
}