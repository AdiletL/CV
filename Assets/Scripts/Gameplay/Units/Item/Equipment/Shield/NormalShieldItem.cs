namespace Gameplay.Unit.Item
{
    public class NormalShieldItem : ShieldItem
    {
        
    }
    
    public class NormalShieldItemBuilder : ShieldItemBuilder
    {
        public NormalShieldItemBuilder() : base(new NormalShieldItem())
        {
        }
    }
}