using System;
using Gameplay.Effect;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Item
{
    public class MadnessMaskItem : Item
    {
        [Inject] private DiContainer diContainer;
        
        public override ItemName ItemNameID { get; protected set; } = ItemName.MadnessMask;

        private VampirismEffect vampirismEffect;
        private VampirismConfig vampirismConfig;
        
        private static readonly string ID = ItemName.MadnessMask.ToString();
        
        public void SetVampirismConfig(VampirismConfig vampirismConfig) => this.vampirismConfig = vampirismConfig;

        public override void Initialize()
        {
            base.Initialize();
            vampirismEffect = new VampirismEffect(vampirismConfig, ID);
            diContainer.Inject(vampirismEffect);
            vampirismEffect.SetTarget(Owner);
        }

        public override void Enter(Action finishedCallBack = null, GameObject target = null, Vector3? point = null)
        {
            base.Enter(finishedCallBack, target, point);
            if(!isActivated) return;
            StartEffect();
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
                effectHandler.RemoveEffect(vampirismEffect.EffectTypeID, vampirismEffect.ID);
        }
    }
    
    public class MadnessMaskItemBuilder : ItemBuilder<MadnessMaskItem>
    {
        public MadnessMaskItemBuilder() : base(new MadnessMaskItem())
        {
        }

        public MadnessMaskItemBuilder SetVampirisimConfig(VampirismConfig config)
        {
            if(item is MadnessMaskItem madnessMask)
                madnessMask.SetVampirismConfig(config);
            return this;
        }
    }
}