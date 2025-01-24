using Unit.Item.Loot;
using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_Loot", menuName = "SO/Gamepley/Unit/Item/Loot", order = 51)]
    public class SO_Loot : SO_Item
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LootType LootType { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
        
        [field: SerializeField, Space(10)] public float JumpPower { get; private set; }
        [field: SerializeField] public float JumpDuration { get; private set; }
    }
}