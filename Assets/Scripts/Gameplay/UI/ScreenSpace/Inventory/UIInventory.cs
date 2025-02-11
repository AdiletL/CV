using Unit.Item;
using UnityEngine;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private UIItemContainer itemContainer;
        

        public void SetMaxCountItem(int value)
        {
            itemContainer.CreateCells(value);
        }

        public void AddItem(ItemData item) => itemContainer.AddItem(item);
        public void RemoveItem(ItemData item) => itemContainer.RemoveItem(item);
    }
}