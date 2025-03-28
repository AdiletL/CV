using System;
using Gameplay.Effect;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Item
{
    public class MadnessMaskItem : Item
    {
        [Inject] private DiContainer diContainer;
        
        public override ItemUsageType ItemUsageTypeID { get; } = ItemUsageType.Nothing;
        public override string ItemName { get; protected set; } = nameof(MadnessMaskItem);

        private VampirismEffect vampirismEffect;
        private VampirismConfig vampirismConfig;
        

        public MadnessMaskItem(SO_MadnessMaskItem so_MadnessMaskItem) : base(so_MadnessMaskItem)
        {
            vampirismConfig = so_MadnessMaskItem.EffectConfigData.VampirismConfig;
        }

        public override void Initialize()
        {
            base.Initialize();
            vampirismEffect = new VampirismEffect(vampirismConfig);
            diContainer.Inject(vampirismEffect);
            vampirismEffect.SetTarget(Owner);
        }
        
        protected override void AfterCast()
        {
            base.AfterCast();
            vampirismEffect.ApplyEffect();
        }

        public override void Exit()
        {
            if(!isActivated) return;
            vampirismEffect.DestroyEffect();
            base.Exit();
        }

        protected override void AddEffectToUnit()
        {
            if (Owner.TryGetComponent(out EffectHandler effectHandler))
                effectHandler.AddEffect(vampirismEffect);
        }

        protected override void RemoveEffectFromUnit()
        {
            if (Owner.TryGetComponent(out EffectHandler effectHandler))
                effectHandler.OnDestroyEffect(vampirismEffect);
        }
    }
}