
namespace Gameplay.Unit.Item
{
    public abstract class SwordItem : EquipmentItem
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.NormalSword;
        

    }

    public abstract class SwordItemBuilder : EquipmentItemBuilder
    {
        protected SwordItemBuilder(Item item) : base(item)
        {
        }
    }
}