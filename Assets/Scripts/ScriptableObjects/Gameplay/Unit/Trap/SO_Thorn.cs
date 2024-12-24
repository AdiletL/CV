using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap
{
    [CreateAssetMenu(fileName = "SO_Thorn", menuName = "SO/Gameplay/Unit/Trap/Thorn", order = 51)]
    public class SO_Thorn : SO_Trap
    {
        [field: SerializeField] public float StartTimer { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float ApplyDamageCooldown { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public LayerMask[] EnemyLayers { get; private set; }
    }
}