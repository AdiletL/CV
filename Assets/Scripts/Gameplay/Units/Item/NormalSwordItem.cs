
namespace Gameplay.Units.Item
{
    public class NormalSwordItem : EquipmentItem
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.NormalSword;
        
    }

    public class NormalSwordItemBuilder : EquipmentItemBuilder
    {
        public NormalSwordItemBuilder() : base(new NormalSwordItem())
        {
        }

    }
}