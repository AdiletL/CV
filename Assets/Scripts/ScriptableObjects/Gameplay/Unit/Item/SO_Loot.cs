using System;
using Unit.Item.Loot;
using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_Loot_", menuName = "SO/Gameplay/Unit/Item/Loot", order = 51)]
    public class SO_Loot : SO_Item
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LootType LootType { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int Amount { get; private set; } = 1;

        [field: SerializeField, Space(10)] public float JumpPower { get; private set; } = 1.5f;
        [field: SerializeField] public float JumpDuration { get; private set; } = 1;
    }
}