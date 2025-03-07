using System;
using Gameplay.Ability;
using Gameplay.Factory;
using UnityEngine;
using Zenject;

namespace Gameplay.Units.Item
{
    public class MadnessMaskItem : Item
    {
        [Inject] private DiContainer diContainer;
        
        public override ItemName ItemNameID { get; protected set; } = ItemName.MadnessMask;

        private VampirismAbility vampirismAbility;
        private VampirismConfig vampirismConfig;
        
        public void SetVampirismConfig(VampirismConfig vampirismConfig) => this.vampirismConfig = vampirismConfig;

        public override void Initialize()
        {
            base.Initialize();
            var abilityFactory = new AbilityFactoryBuilder()
                .SetOwner(OwnerGameObject)
                .Build();
            diContainer.Inject(abilityFactory);
            
            vampirismAbility = (VampirismAbility)abilityFactory.CreateAbility(vampirismConfig);
            diContainer.Inject(vampirismAbility);
            vampirismAbility.Initialize();
            AddAbility(vampirismAbility);
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
            vampirismAbility.Enter();
        }

        public override void Exit()
        {
            if(!isActivated) return;
            vampirismAbility.Exit();
            base.Exit();
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