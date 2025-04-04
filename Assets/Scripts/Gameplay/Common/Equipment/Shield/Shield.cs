using ScriptableObjects.Gameplay.Equipment;

namespace Gameplay.Equipment.Shield
{
    public class Shield : Equipment
    {
        public override EquipmentType EquipmentTypeID { get; protected set; } = EquipmentType.Shield;
        
        public Shield(SO_Equipment so_Equipment) : base(so_Equipment)
        {
        }

    }
}