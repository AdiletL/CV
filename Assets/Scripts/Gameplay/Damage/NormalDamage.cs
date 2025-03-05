using Calculate;
using Gameplay.Resistance;
using Gameplay.Spawner;
using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Damage
{
    public class NormalDamage : Damage
    {
        public NormalDamage(GameObject gameObject, Stat damageStat) : base(gameObject, damageStat)
        {

        }
    }
}