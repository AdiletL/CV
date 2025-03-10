﻿using System.Collections.Generic;
using Gameplay.Ability;
using Gameplay.Unit.Item;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using Zenject;

namespace Gameplay.Factory
{
    public class InventoryItemFactory : Factory
    {
        [Inject] private DiContainer diContainer;
        
        private GameObject owner;
        private Camera baseCamera;

        public void SetOwner(GameObject gameObject) => this.owner = gameObject;
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
            return (MadnessMaskItem)new MadnessMaskItemBuilder()
                .SetVampirisimConfig(so_MadnessMask.AbilityConfigData.VampirismConfig)
                .SetItemBehaviour(so_MadnessMask.ItemBehaviourID)
                .SetItemCategory(so_MadnessMask.ItemCategoryID)
                .SetBlockInput(so_MadnessMask.BlockInputTypeID)
                .SetTimerCast(so_MadnessMask.TimerCast)
                .SetCooldown(so_MadnessMask.Cooldown)
                .SetGameObject(owner)
                .Build();
        }

        private TeleportationScrollItem CreateTeleportationScroll(SO_Item so_Item)
        {
            var so_TeleportationScroll = so_Item as SO_TeleportationScroll;
            return (TeleportationScrollItem)new TeleportationScrollItemBuilder()
                .SetPortalObject(so_TeleportationScroll.PortalObject)
                .SetEndPortalID(so_TeleportationScroll.EndPortalID.ID)
                .SetGameObject(owner)
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
                .SetGameObject(owner)
                .Build();
        }
    }

    public class ItemInventoryFactoryBuilder
    {
        private InventoryItemFactory inventoryItemFactory = new ();
        
        public InventoryItemFactory Build()
        {
            return inventoryItemFactory;
        }
    }
}