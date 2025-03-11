using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap
{
    public abstract class SO_Trap : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; protected set; }
        [field: SerializeField] public AnimationClip AppearClip { get; protected set; }
        [field: SerializeField] public AnimationClip DeappearClip { get; protected set; }
        [field: SerializeField] public LayerMask EnemyLayer { get; private set; }
    }
}