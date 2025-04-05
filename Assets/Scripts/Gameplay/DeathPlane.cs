using System;
using UnityEngine;

namespace Gameplay
{
    public class DeathPlane : MonoBehaviour
    {
        private DamageData damageData;

        private void Start()
        {
            damageData = new DamageData(gameObject, DamageType.Physical, 999999, false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IAttackable attackable) &&
                other.TryGetComponent(out IHealth health) &&
                health.IsLive)
            {
                attackable.TakeDamage(damageData);
            }
            else if (!other.TryGetComponent(out IAttackable attackable2))
            {
                other.gameObject.SetActive(false);
            }
        }
    }
}