using System.Collections.Generic;
using Gameplay.Ability;
using Gameplay.Units.Item;
using ScriptableObjects.Equipment.Weapon;
using ScriptableObjects.Gameplay.Equipment;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using Zenject;

namespace Gameplay.Factory
{
    public class ItemFactory : Factory
    {
        [Inject] private DiContainer diContainer;
        
        private AbilityFactory abilityFactory;
        private GameObject gameObject;
        private Camera baseCamera;

        public void SetAbilityFactory(AbilityFactory abilityFactory) => this.abilityFactory = abilityFactory;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetBaseCamera(Camera camera) => this.baseCamera = camera;

        public void Initialize()
        {
            
        }

        public Item CreateItem(SO_Item so_Item)
        {
            Item result = so_Item switch
            {
                _ when so_Item.GetType() == typeof(SO_MadnessMask) => CreateMadnessMask(so_Item),
                _ when so_Item.GetType() == typeof(SO_TeleportationScroll) => CreateTeleportationScroll(so_Item),
                _ when so_Item.GetType() == typeof(SO_NormalSword) => CreateNormalSword(so_Item),
            };
            return result;
        }

        private MadnessMaskItem CreateMadnessMask(SO_Item so_Item)
        {
            var so_MadnessMask = so_Item as SO_MadnessMask;
            var vampirismAbility = (VampirismAbility)abilityFactory.CreateAbility(so_MadnessMask.AbilityConfigData.VampirismConfig);
            diContainer.Inject(vampirismAbility);
            
            return (MadnessMaskItem)new MadnessMaskItemBuilder()
                .SetVampirismAbility(vampirismAbility)
                .SetItemBehaviour(so_MadnessMask.ItemBehaviourID)
                .SetItemCategory(so_MadnessMask.ItemCategoryID)
                .SetBlockInput(so_MadnessMask.BlockInputTypeID)
                .SetTimerCast(so_MadnessMask.TimerCast)
                .SetCooldown(so_MadnessMask.Cooldown)
                .SetGameObject(gameObject)
                .Build();
        }

        private TeleportationScrollItem CreateTeleportationScroll(SO_Item so_Item)
        {
            var so_TeleportationScroll = so_Item as SO_TeleportationScroll;
            return (TeleportationScrollItem)new TeleportationScrollItemBuilder()
                .SetSelectTargetCursor(so_TeleportationScroll.SelectTargetCursor)
                .SetPortalObject(so_TeleportationScroll.PortalObject)
                .SetEndPortalID(so_TeleportationScroll.EndPortalID.ID)
                .SetBaseCamera(baseCamera)
                .SetGameObject(gameObject)
                .SetTimerCast(so_TeleportationScroll.TimerCast)
                .SetCooldown(so_TeleportationScroll.Cooldown)
                .SetBlockInput(so_TeleportationScroll.BlockInputTypeID)
                .SetItemBehaviour(so_TeleportationScroll.ItemBehaviourID)
                .SetItemCategory(so_TeleportationScroll.ItemCategoryID)
                .Build();
        }

        private NormalSwordItem CreateNormalSword(SO_Item so_Item)
        {
            var so_NormalSword = so_Item as SO_NormalSword;
            return (NormalSwordItem)new NormalSwordItemBuilder()
                .SetEquipmentConfig(so_NormalSword.SO_Equipment)
                .SetTimerCast(so_Item.TimerCast)
                .SetItemBehaviour(so_Item.ItemBehaviourID)
                .SetBlockInput(so_Item.BlockInputTypeID)
                .SetCooldown(so_NormalSword.Cooldown)
                .SetGameObject(gameObject)
                .Build();
        }
    }

    public class ItemFactoryBuilder
    {
        private ItemFactory itemFactory = new ();

        public ItemFactoryBuilder SetAbilityFactory(AbilityFactory abilityFactory)
        {
            this.itemFactory.SetAbilityFactory(abilityFactory);
            return this;
        }
        
        public ItemFactoryBuilder SetGameObject(GameObject gameObject)
        {
            this.itemFactory.SetGameObject(gameObject);
            return this;
        }
        
        public ItemFactoryBuilder SetBaseCamera(Camera baseCamera)
        {
            this.itemFactory.SetBaseCamera(baseCamera);
            return this;
        }
        
        public ItemFactory Build()
        {
            return itemFactory;
        }
    }
}