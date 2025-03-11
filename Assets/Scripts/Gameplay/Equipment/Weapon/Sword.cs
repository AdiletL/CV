using Gameplay.Unit;
using Unit;
using UnityEngine;

namespace Gameplay.Equipment.Weapon
{
    public class Sword : Weapon
    {
        private Collider[] findColliders = new Collider[1];
        private int ownerLayer;
        
        public override void Initialize()
        {
            base.Initialize();
            ownerLayer = Owner.layer;
        }

        private void FindUnitInRange()
        {
            var totalRange = RangeStat.CurrentValue + OwnerRangeStat.CurrentValue;
            var target = Calculate.Attack.FindUnitInRange<IAttackable>(ownerCenter.position, totalRange,
                enemyLayer, ref findColliders);
            if(!target) return;
            
            var directionToTarget = (target.GetComponent<UnitCenter>().Center.position - ownerCenter.position).normalized;
            //Debug.DrawRay(origin, directionToTarget * 100, Color.green, 2);
            if (Physics.Raycast(ownerCenter.position, directionToTarget, out var hit, totalRange, ~ownerLayer))
            {
                if(hit.collider.gameObject.layer == target.gameObject.layer)
                    currentTarget = target;
            }
        }
        
        public override void ApplyDamage()
        {
            FindUnitInRange();
            if(currentTarget &&
               Calculate.Rotate.IsFacingTargetXZ(Owner.transform.position,
                   Owner.transform.forward, currentTarget.transform.position, angleToTarget) &&
               currentTarget.TryGetComponent(out IAttackable attackable) && 
               currentTarget.TryGetComponent(out IHealth health) && health.IsLive)
            {
                DamageData.Amount = DamageStat.CurrentValue + OwnerDamageStat.CurrentValue;
                attackable.TakeDamage(DamageData);
            }

            currentTarget = null;
        }
    }

    public class SwordBuilder : WeaponBuilder
    {
        public SwordBuilder() : base(new Sword())
        {
        }
    }
}