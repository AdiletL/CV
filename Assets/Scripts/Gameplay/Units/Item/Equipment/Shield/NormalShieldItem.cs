using Gameplay.Equipment;

namespace Gameplay.Unit.Item
{
    public class NormalShieldItem : ShieldItem
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.NormalShield;
        public override EquipmentType EquipmentTypeID { get; protected set; } = EquipmentType.Shield;
    }
    
    public class NormalShieldItemBuilder : ShieldItemBuilder
    {
        public NormalShieldItemBuilder() : base(new NormalShieldItem())
        {
        }
    }
}