using Gameplay.Unit.Item;
using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_NormalSwordItem", menuName = "SO/Gameplay/Item/Equipment/Weapon/NormalSword", order = 51)]
    public class SO_NormalSwordItem : SO_EquipmentItem
    {
        public override string ItemName { get; protected set; } = nameof(NormalSwordItem);
    }
}