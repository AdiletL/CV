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

        public void AddItem(ItemData item) => itemContainer.AddItem(item);
        public void UpdateItem(ItemData item) => itemContainer.UpdateItem(item);
        public void UpdateItemCooldown(int? slotID, float current, float max) => itemContainer.UpdateItemCooldown(slotID, current, max);
        public void ChangeReadiness(int? slotID, bool isReady) => itemContainer.ChangeReadiness(slotID, isReady);
        public void RemoveItem(int? slotID) => itemContainer.RemoveItem(slotID);
    }
}