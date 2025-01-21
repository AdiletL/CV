using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap
{    
    [CreateAssetMenu(fileName = "SO_Push", menuName = "SO/Gameplay/Unit/Trap/Push", order = 51)]
    public class SO_Push : SO_Trap
    {
        [field: SerializeField] public int AttackSpeed { get; private set; } = 100;
        [field: SerializeField] public float CooldownAttack { get; private set; }
    }
}