using Gameplay.Equipment;
using ScriptableObjects.Unit.Item;

namespace Gameplay.Unit.Item
{
    public class NormalShieldItem : ShieldItem
    {
        public override string ItemName { get; protected set; } = nameof(NormalShieldItem);
        public NormalShieldItem(SO_NormalShieldItem so_EquipmentItem) : base(so_EquipmentItem)
        {
        }

    }
}