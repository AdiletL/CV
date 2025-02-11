using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unit.Item;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.UI.ScreenSpace.Inventory
{
    public class UIItemContainer : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject uiItemPrefab;
        [SerializeField] private Transform container;
        
        private List<UIItem> uiItems = new ();
        
        public async void CreateCells(int maxCountItem)
        {
            UIItem uiItem = null;
            for (int i = 0; i < maxCountItem; i++)
            {
                var handle = await Addressables.InstantiateAsync(uiItemPrefab, container);
                uiItem = handle.GetComponent<UIItem>();
                uiItem.Initialize();
                uiItem.Clear();
                uiItems.Add(uiItem);
            }
        }

        public void AddItem(ItemData data)
        {
            UIItem emptySlot = null;

            foreach (var uiItem in uiItems)
            {
                if (string.Equals(data.Name, uiItem.Name, StringComparison.Ordinal))
                {
                    uiItem.SetValue(data.Amount);
                    return;
                }

                if (emptySlot == null && string.IsNullOrEmpty(uiItem.Name))
                {
                    emptySlot = uiItem; // Запоминаем первый пустой слот
                }
            }

            if(emptySlot == null) return;
            
            emptySlot.Name = data.Name;
            emptySlot.SetIcon(data.Icon);
            emptySlot.SetValue(data.Amount);
            emptySlot.SetCanSelect(data.IsCanSelect);
        }

        public void RemoveItem(ItemData data)
        {
            foreach (var uiItem in uiItems)
            {
                if (string.Equals(data.Name, uiItem.Name, StringComparison.Ordinal))
                {
                    uiItem.SetValue(data.Amount);
                    return;
                }
            }
        }
    }
}