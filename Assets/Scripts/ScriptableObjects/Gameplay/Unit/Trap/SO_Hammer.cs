using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap
{
    [CreateAssetMenu(fileName = "SO_Hammer", menuName = "SO/Gameplay/Unit/Trap/Hammer", order = 51)]
    public class SO_Hammer : SO_Trap
    {
        [field: SerializeField] public int AttackSpeed { get; private set; } = 100;
        [field: SerializeField] public float CooldownAttack { get; private set; }
    }
}