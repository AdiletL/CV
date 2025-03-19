using System;
using UnityEngine;

namespace Gameplay.Unit.Item
{
    public abstract class ShieldItem : EquipmentItem
    {
        
    }

    public abstract class ShieldItemBuilder : EquipmentItemBuilder
    {
        protected ShieldItemBuilder(Item item) : base(item)
        {
        }
    }
}