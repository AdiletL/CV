using Gameplay.Effect;
using UnityEngine;

namespace ScriptableObjects.Weapon.Projectile
{
    [CreateAssetMenu(fileName = "SO_Sphere", menuName = "SO/Gameplay/Weapon/Projectile/Sphere", order = 51)]
    public class SO_Sphere : SO_Projectile
    {
        [field: SerializeField] public SlowMovementInfo SlowMovementInfo { get; set; }
    }
}