using System;
using Gameplay.Equipment;
using ScriptableObjects.Unit.Item;
using UnityEngine;

namespace Gameplay.Unit.Item
{
    public abstract class ShieldItem : EquipmentItem
    {
        public override EquipmentType EquipmentTypeID { get; protected set; } = EquipmentType.Shield;

        protected ShieldItem(SO_EquipmentItem so_EquipmentItem) : base(so_EquipmentItem)
        {
        }
    }
}