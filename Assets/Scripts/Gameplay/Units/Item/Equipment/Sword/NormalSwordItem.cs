using Gameplay.AttackModifier;
using ScriptableObjects.Unit.Item;

namespace Gameplay.Unit.Item
{
    public class NormalSwordItem : SwordItem, ICriticalDamageApplier
    {
        public override string ItemName { get; protected set; } = nameof(NormalSwordItem);
        public ICriticalDamage CriticalDamage { get; }
        
        public NormalSwordItem(SO_NormalSwordItem so_EquipmentItem) : base(so_EquipmentItem)
        {
            var criticalDamageConfig = so_EquipmentItem.AttackModifierConfigData.CriticalDamageConfig;
            CriticalDamage = new CriticalDamage(
                criticalDamageConfig.GameValueConfig.Value, 
                criticalDamageConfig.GameValueConfig.ValueTypeID,
                criticalDamageConfig.Chance);
        }

        protected override void AfterCast()
        {
            base.AfterCast();
            CriticalDamage.Activate();
        }

        public override void TakeOff()
        {
            base.TakeOff();
            CriticalDamage.Deactivate();
        }
    }
}