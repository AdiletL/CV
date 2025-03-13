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

        private GameObject FindUnitInRange()
        {
            var totalRange = RangeStat.CurrentValue + OwnerRangeStat.CurrentValue;
            var target = Calculate.Attack.FindUnitInRange<IAttackable>(ownerCenter.position, totalRange,
                enemyLayer, ref findColliders);
            if(!target) return null;

            if (!isObstacleBetween(target))
                return target;

            return null;
        }

        private bool isObstacleBetween(GameObject target)
        {
            var directionToTarget = (target.GetComponent<UnitCenter>().Center.position - ownerCenter.position).normalized;
            float distance = Vector3.Distance(ownerCenter.position, target.transform.position);
            int ignoreLayer = 1 << ownerLayer;
            //Debug.DrawRay(ownerCenter.position, directionToTarget * totalRange, Color.green, 2);
            if (Physics.Raycast(ownerCenter.position,  directionToTarget, out var hitInTarget, distance, ~ignoreLayer))
            {
                if(hitInTarget.collider.gameObject.layer == target.gameObject.layer)
                    return false;
            }
            return true;
        }
        
        public override void ApplyDamage()
        {
            currentTarget = FindUnitInRange();
            if(currentTarget &&
               !isObstacleBetween(currentTarget) &&
               Calculate.Rotate.IsFacingTargetXZ(Owner.transform.position,
                   Owner.transform.forward, currentTarget.transform.position, angleToTarget) &&
               Calculate.Rotate.IsFacingTargetY(Owner.transform.position, currentTarget.transform.position, 50) &&
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