using UnityEngine;

namespace ScriptableObjects.Weapon.Projectile
{
    public abstract class SO_Projectile : ScriptableObject
    {
        [field: SerializeField] public AnimationCurve Curve { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Height { get; private set; }
        [field: SerializeField] public LayerMask EnemyLayer { get; private set; }
    }
}