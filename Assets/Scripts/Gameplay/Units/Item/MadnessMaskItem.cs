using System;
using Gameplay.Ability;
using UnityEngine;

namespace Gameplay.Units.Item
{
    public class MadnessMaskItem : Item
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.MadnessMask;

        private VampirismAbility vampirismAbility;
        
        public void SetApplyDamageHeal(VampirismAbility vampirismAbility) => this.vampirismAbility = vampirismAbility;

        public override void Initialize()
        {
            base.Initialize();
            vampirismAbility.Initialize();
            AddAbility(vampirismAbility);
        }

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
            vampirismAbility.Enter();
        }

        public override void Exit()
        {
            vampirismAbility.Exit();
            base.Exit();
        }
    }
    
    public class MadnessMaskItemBuilder : ItemBuilder<MadnessMaskItem>
    {
        public MadnessMaskItemBuilder() : base(new MadnessMaskItem())
        {
        }

        public MadnessMaskItemBuilder SetVampirismAbility(VampirismAbility vampirismAbility)
        {
            if(item is MadnessMaskItem madnessMask)
                madnessMask.SetApplyDamageHeal(vampirismAbility);
            return this;
        }
    }
}