namespace Gameplay.Unit.Item
{
    public class NormalSwordItem : SwordItem
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.NormalSword;

    }
    
    public class NormalSwordItemBuilder : SwordItemBuilder
    {
        public NormalSwordItemBuilder() : base(new NormalSwordItem())
        {
        }
    }
}