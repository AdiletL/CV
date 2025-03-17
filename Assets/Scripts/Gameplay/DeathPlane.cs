using System;
using UnityEngine;

namespace Gameplay
{
    public class DeathPlane : MonoBehaviour
    {
        private DamageData damageData;
        private Stat damageStat = new Stat();

        private void Start()
        {
            damageStat.AddCurrentValue(999999);
            damageData = new DamageData(gameObject, DamageType.Physical, damageStat.CurrentValue);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IAttackable attackable) &&
                other.TryGetComponent(out IHealth health) &&
                health.IsLive)
            {
                attackable.TakeDamage(damageData);
            }
        }
    }
}