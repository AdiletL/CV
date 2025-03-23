using DG.Tweening;
using Gameplay.Factory;
using Gameplay.UI;
using Gameplay.Unit.Character.Player;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Item
{
    public class ItemController : UnitController
    {
        [Inject] private InventoryItemFactory inventoryItemFactory;
        
        [SerializeField] private SO_Item so_Item;
        
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
                    playerInventory.IsNotNullItem(so_Item.ItemName))
                {
                    var item = inventoryItemFactory.CreateItem(so_Item);
                    diContainer.Inject(item);
                    item.SetOwner(playerInventory.gameObject);
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