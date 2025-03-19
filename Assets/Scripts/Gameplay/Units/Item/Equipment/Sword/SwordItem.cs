
using Gameplay.Equipment;

namespace Gameplay.Unit.Item
{
    public abstract class SwordItem : EquipmentItem
    {
        public override EquipmentType EquipmentTypeID { get; protected set; } = EquipmentType.Weapon;
    }

    public abstract class SwordItemBuilder : EquipmentItemBuilder
    {
        protected SwordItemBuilder(Item item) : base(item)
        {
        }
    }
}