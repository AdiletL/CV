namespace Gameplay.Unit.Item
{
    public class NormalSwordItem : SwordItem
    {
        
    }
    
    public class NormalSwordItemBuilder : SwordItemBuilder
    {
        public NormalSwordItemBuilder() : base(new NormalSwordItem())
        {
        }
    }
}