using System;
using Gameplay.Unit.Item;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using Zenject;

namespace Gameplay.Factory
{
    public class InventoryItemFactory : Factory
    {
        [Inject] private DiContainer diContainer;
        
        public Item CreateItem(SO_Item so_Item)
        {
            Item result = so_Item switch
            {
                _ when so_Item.GetType() == typeof(SO_MadnessMaskItem) => CreateMadnessMask(so_Item),
                _ when so_Item.GetType() == typeof(SO_TeleportationScrollItem) => CreateTeleportationScroll(so_Item),
                _ when so_Item.GetType() == typeof(SO_NormalSwordItem) => CreateNormalSword(so_Item),
                _ when so_Item.GetType() == typeof(SO_NormalShieldItem) => CreateNormalShield(so_Item),
                _ => throw new ArgumentOutOfRangeException(nameof(so_Item), so_Item, null)
            };
            return result;
        }

        private MadnessMaskItem CreateMadnessMask(SO_Item so_Item)
        {
            return new MadnessMaskItem(so_Item as SO_MadnessMaskItem);
        }

        private TeleportationScrollItem CreateTeleportationScroll(SO_Item so_Item)
        {
            return new TeleportationScrollItem(so_Item as SO_TeleportationScrollItem);
        }

        private NormalSwordItem CreateNormalSword(SO_Item so_Item)
        {
            return new NormalSwordItem(so_Item as SO_NormalSwordItem);
        }

        private NormalShieldItem CreateNormalShield(SO_Item so_Item)
        {
            return new NormalShieldItem(so_Item as SO_NormalShieldItem);
        }
    }
}