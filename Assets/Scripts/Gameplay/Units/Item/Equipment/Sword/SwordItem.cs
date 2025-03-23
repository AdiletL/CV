using Gameplay.Equipment;
using ScriptableObjects.Unit.Item;

namespace Gameplay.Unit.Item
{
    public abstract class SwordItem : EquipmentItem
    {
        public override EquipmentType EquipmentTypeID { get; protected set; } = EquipmentType.Weapon;
        protected SwordItem(SO_EquipmentItem so_EquipmentItem) : base(so_EquipmentItem)
        {
            
        }
    }
}