using System.Collections.Generic;
using DG.Tweening;
using Gameplay.Ability;
using Gameplay.Factory;
using Gameplay.UI;
using ScriptableObjects.Unit.Item;
using Unit.Character.Player;
using UnityEngine;

namespace Unit.Item
{
    public class ItemController : UnitController
    {
        [SerializeField] private SO_Item so_Item;
        
        private InventoryItemFactory inventoryItemFactory;
        private HotkeyUI hotkeyUI;
        private int amountItem;
        
        private float jumpPower;
        private float jumpDuration;

        private bool isSelected;

        public bool IsCanTake { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            jumpPower = so_Item.JumpPower;
            jumpDuration = so_Item.JumpDuration;
            
            inventoryItemFactory = new ItemInventoryFactoryBuilder()
                .Build();
            diContainer.Inject(inventoryItemFactory);
            inventoryItemFactory.Initialize();
            
            hotkeyUI = GetComponentInUnit<PressHotkeyUI>();
            Disable();
        }

        public override void Appear()
        {
            
        }

        public override void Disappear()
        {
            throw new System.NotImplementedException();
        }

        public void SetAmount(int amount) => this.amountItem = amount;
        
        public void TakeItem(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out PlayerItemInventory playerInventory))
            {
                if (!playerInventory.IsFullInventory() || 
                    playerInventory.IsNotNullItem(so_Item.ItemNameID))
                {
                    inventoryItemFactory.SetOwner(playerInventory.gameObject);
                    var item = inventoryItemFactory.CreateItem(so_Item);
                    diContainer.Inject(item);
                    item.SetAmountItem(amountItem);
                    item.SetStats(so_Item.UnitStatsConfigs);
                    item.Initialize();
                    playerInventory.AddItem(item, so_Item.Icon);
                    Destroy(this.gameObject);
                }
            }
        }

        public void Enable(KeyCode takeKey)
        {
            isSelected = true;
            hotkeyUI.SetText(takeKey.ToString());
            hotkeyUI.Show();
        }

        public void Disable()
        {
            hotkeyUI.Hide();
            isSelected = false;
        }

        public void JumpToPoint(Vector3 point)
        {
            transform.DOJump(point, jumpPower, 1, jumpDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    IsCanTake = true;
                    if(isSelected) hotkeyUI.Show();
                });
        }
    }
}