using System;
using Gameplay.Damage;
using UnityEngine;

namespace Gameplay
{
    public class DeathPlane : MonoBehaviour
    {
        private IDamageable damageable;

        private void Start()
        {
            damageable = new NormalDamage(-1, gameObject);
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