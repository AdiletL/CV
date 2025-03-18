using Gameplay.Unit;
using Unit;
using UnityEngine;

namespace Gameplay.Equipment.Weapon
{
    public class Sword : Weapon
    {
        private Collider[] findColliders = new Collider[1];
        private int ownerLayer;
        private int counterSpecialAction;
        private const int SPECIAL_ATTACK_INDEX = 2;
        
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
            if(!target) return true;
            
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
            if (IsActivatedSpecialAction)
            {
                SpecialAction();
            }
            else
            {
                currentTarget = FindUnitInRange();
                if (currentTarget &&
                    Calculate.Rotate.IsFacingTargetXZ(Owner.transform.position,
                        Owner.transform.forward, currentTarget.transform.position, angleToTarget) &&
                    currentTarget.TryGetComponent(out IAttackable attackable) &&
                    currentTarget.TryGetComponent(out IHealth health) && health.IsLive)
                {
                    DamageData.Amount = DamageStat.CurrentValue + OwnerDamageStat.CurrentValue;
                    attackable.TakeDamage(DamageData);

                    if (counterSpecialAction >= SPECIAL_ATTACK_INDEX)
                    {
                        IsActivatedSpecialAction = true;
                        SpecialActionIndex = 0;
                        counterSpecialAction = 0;
                    }
                    else
                    {
                        counterSpecialAction++;
                    }
                }

                currentTarget = null;
            }
        }

        private void SpecialAction()
        {
            var totalRange = RangeStat.CurrentValue + OwnerRangeStat.CurrentValue;
            var colliders = Physics.OverlapSphere(ownerCenter.position, totalRange, enemyLayer);
            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                var target = colliders[i]?.gameObject;
                if(!target) continue;

                if (!isObstacleBetween(target) &&
                    target.TryGetComponent(out IAttackable attackable) &&
                    target.TryGetComponent(out IHealth health) && health.IsLive)
                {
                    DamageData.Amount = DamageStat.CurrentValue + OwnerDamageStat.CurrentValue;
                    attackable.TakeDamage(DamageData);
                }
            }
            IsActivatedSpecialAction = false;
        }
    }

    public class SwordBuilder : WeaponBuilder
    {
        public SwordBuilder() : base(new Sword())
        {
        }
    }
}