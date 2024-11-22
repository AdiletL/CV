using UnityEngine;

namespace ScriptableObjects.Weapon.Projectile
{
    public abstract class SO_Projectile : ScriptableObject
    {
        [field: SerializeField] public AnimationCurve curve { get; private set; }
        [field: SerializeField] public float speed { get; private set; }
    }
}