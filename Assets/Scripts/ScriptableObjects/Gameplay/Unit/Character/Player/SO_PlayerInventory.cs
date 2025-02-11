using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerInventory", menuName = "SO/Gameplay/Unit/Character/Player/Inventory", order = 51)]
    public class SO_PlayerInventory : ScriptableObject
    {
        [field: SerializeField] public int MaxCountItem { get; private set; } = 1;
    }
}