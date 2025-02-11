using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay.Weapon
{
    public class Sword : Weapon
    {
        protected Collider[] findUnitColliders = new Collider[10];
        
        private GameObject FindUnit()
        {
            return Calculate.Attack.FindUnitInRange(ownerCenter.position, Range, enemyLayer, ref findUnitColliders);
        }
        
        public override async UniTask FireAsync()
        {
            CurrentTarget = FindUnit();
            
            if(CurrentTarget &&
               Calculate.Move.IsFacingTargetUsingAngle(gameObject.transform.position,
                   gameObject.transform.forward, CurrentTarget.transform.position, angleToTarget))
            {
                ApplyDamage();
                return;
            }

            await UniTask.CompletedTask;
        }
    }

    public class SwordBuilder : WeaponBuilder
    {
        public SwordBuilder() : base(new Sword())
        {
        }
    }
}