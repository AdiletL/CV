using Gameplay;
using Gameplay.Unit.Trap;
using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap
{
    public abstract class SO_Trap : ScriptableObject
    {
        [field: SerializeField] public float Damage { get; protected set; }
        [field: SerializeField] public float ActivationDelay { get; protected set; }
        [field: SerializeField] public float Cooldown { get; private set; }

        [field: SerializeField] public bool IsReusable  { get; protected set; }
        [field: SerializeField, Space] public AnimationClip PlayClip { get; protected set; }
        [field: SerializeField] public AnimationClip ResetClip { get; protected set; }
        [field: SerializeField, Space] public LayerMask EnemyLayer { get; private set; }
    }
}