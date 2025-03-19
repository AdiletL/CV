using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_NormalSword", menuName = "SO/Gameplay/Item/Equipment/Weapon/NormalSword", order = 51)]
    public class SO_NormalSword : SO_EquipmentItem
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.NormalSword;
    }
}