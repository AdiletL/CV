using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterBaseAttackState : UnitBaseAttackState
    {
        protected GameObject currentTarget;
        
        public override void Initialize()
        {
            
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