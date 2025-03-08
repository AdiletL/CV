using System;
using Gameplay.Damage;
using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class DeathPlane : MonoBehaviour
    {
        private IDamageable damageable;
        private Stat damageStat = new Stat();

        private void Start()
        {
            damageStat.AddValue(999999);
            damageable = new NormalDamage(gameObject, damageStat.CurrentValue);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IAttackable attackable) &&
                other.TryGetComponent(out IHealth health) &&
                health.IsLive)
            {
                attackable.TakeDamage(damageable);
            }
        }
    }
}