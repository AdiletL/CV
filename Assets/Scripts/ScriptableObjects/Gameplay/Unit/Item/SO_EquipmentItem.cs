using ScriptableObjects.Equipment.Weapon;
using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    public abstract class SO_EquipmentItem : SO_Item
    {
        [field: SerializeField, Space(15)] public Gameplay.Equipment.SO_Equipment SO_Equipment { get; private set; }

    }
}