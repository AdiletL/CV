using Unit.Item;
using UnityEngine;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIItemInventory : UIInventory
    {
        [SerializeField] private UIItemContainer itemContainer;
        

        public override void SetMaxCountCells(int value)
        {
            itemContainer.CreateCells(value);
        }

        public void AddItem(int? slotID, Sprite icon, int amount, bool isSelectable, float currentCooldown, float maxCooldown) => itemContainer.AddItem(slotID, icon, amount, isSelectable, currentCooldown, maxCooldown);
        public void UpdateAmount(int? slotID, int amount) => itemContainer.UpdateAmount(slotID, amount);
        public void UpdateItemCooldown(int? slotID, float current, float max) => itemContainer.UpdateItemCooldown(slotID, current, max);
        public void UpdateSelectable(int? slotID, bool isSelectable) => itemContainer.UpdateSelectable(slotID, isSelectable);
        public void RemoveItem(int? slotID) => itemContainer.RemoveItem(slotID);
    }
}