using Gameplay.Unit.Item;
using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_NormalShieldItem", menuName = "SO/Gameplay/Item/Equipment/Shield/Normal", order = 51)]
    public class SO_NormalShieldItem : SO_EquipmentItem
    {
        public override string ItemName { get; protected set; } = nameof(NormalShieldItem);
    }
}