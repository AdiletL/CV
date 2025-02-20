using System;
using Gameplay.Ability;
using UnityEngine;

namespace Gameplay.Units.Item
{
    public class MadnessMask : Item
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.MadnessMask;


        private ApplyDamageHeal applyDamageHeal;
        
        public void SetApplyDamageHeal(ApplyDamageHeal applyDamageHeal) => this.applyDamageHeal = applyDamageHeal;

        public override void Initialize()
        {
            base.Initialize();
            applyDamageHeal.Initialize();
            AddAbility(applyDamageHeal);
        }

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
            applyDamageHeal.Enter();
        }

        public override void Exit()
        {
            applyDamageHeal.Exit();
            base.Exit();
        }
    }
    
    public class MadnessMaskBuilder : ItemBuilder<MadnessMask>
    {
        public MadnessMaskBuilder() : base(new MadnessMask())
        {
        }

        public MadnessMaskBuilder SetApplyDamageHeal(ApplyDamageHeal applyDamageHeal)
        {
            if(item is MadnessMask madnessMask)
                madnessMask.SetApplyDamageHeal(applyDamageHeal);
            return this;
        }
    }
}