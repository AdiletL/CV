namespace Gameplay.Unit.Item
{
    public class NormalShieldItem : EquipmentItem
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.NormalShield;
    }

    public class NormalShieldItemBuilder : EquipmentItemBuilder
    {
        public NormalShieldItemBuilder() : base(new NormalShieldItem())
        {
        }
    }
}