using System;
using UnityEngine;

namespace Gameplay.Unit.Item
{
    public abstract class ShieldItem : EquipmentItem
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.NormalShield;

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
        }
        
        
    }

    public abstract class ShieldItemBuilder : EquipmentItemBuilder
    {
        protected ShieldItemBuilder(Item item) : base(item)
        {
        }
    }
}