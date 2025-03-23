using ScriptableObjects.Unit.Item;

namespace Gameplay.Unit.Item
{
    public class NormalSwordItem : SwordItem
    {
        public override string ItemName { get; protected set; } = nameof(NormalSwordItem);
        public NormalSwordItem(SO_NormalSwordItem so_EquipmentItem) : base(so_EquipmentItem)
        {
        }

    }

}