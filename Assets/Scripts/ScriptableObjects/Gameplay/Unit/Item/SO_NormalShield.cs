using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_NormalShield", menuName = "SO/Gameplay/Item/Equipment/Shield/Normal", order = 51)]
    public class SO_NormalShield : SO_EquipmentItem
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.NormalShield;
    }
}