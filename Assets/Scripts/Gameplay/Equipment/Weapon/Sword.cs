using Gameplay.Damage;
using Unit;
using UnityEngine;

namespace Gameplay.Equipment.Weapon
{
    public class Sword : Weapon
    {
        private Collider[] findColliders = new Collider[1];
        private int ownerLayer;
        
        protected override IDamageable CreateDamageable()
        {
            return new NormalDamage(owner, DamageStat);
        }

        public override void Initialize()
        {
            base.Initialize();
            ownerLayer = owner.layer;
        }

        private void FindUnitInRange()
        {
            var target = Calculate.Attack.FindUnitInRange(ownerCenter.position, RangeStat.CurrentValue,
                enemyLayer, ref findColliders);
            if(target == null) return;
            
            var directionToTarget = (target.GetComponent<UnitCenter>().Center.position - ownerCenter.position).normalized;
            
            //Debug.DrawRay(origin, directionToTarget * 100, Color.green, 2);
            if (Physics.Raycast(ownerCenter.position, directionToTarget, out var hit, RangeStat.CurrentValue, ~ownerLayer))
            {
                if(hit.collider.gameObject.layer == target.gameObject.layer)
                    currentTarget = target;
            }
        }
        
        public override void ApplyDamage()
        {
            FindUnitInRange();
            if(currentTarget &&
               Calculate.Rotate.IsFacingTargetUsingAngle(owner.transform.position,
                   owner.transform.forward, currentTarget.transform.position, angleToTarget) &&
               currentTarget.TryGetComponent(out IAttackable attackable) && 
               currentTarget.TryGetComponent(out IHealth health) && health.IsLive)
            {
                attackable.TakeDamage(Damageable);
            }
        }
    }

    public class SwordBuilder : WeaponBuilder
    {
        public SwordBuilder() : base(new Sword())
        {
        }
    }
}