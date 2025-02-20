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
        
        private Dictionary<int?, UIItem> uiItems = new ();
        
        public void CreateCells(int maxCountItem)
        {
            UIItem uiItem = null;
            for (int i = 0; i < maxCountItem; i++)
            {
                var handle = Addressables.InstantiateAsync(uiItemPrefab, container).WaitForCompletion();
                uiItem = handle.GetComponent<UIItem>();
                uiItem.Initialize();
                uiItem.Clear();
                uiItems.Add(i, uiItem);
            }
        }

        public void AddItem(int? slotID, Sprite icon, int amount, bool isReady, float currentCooldown, float maxCooldown)
        {
            if(slotID == null) return;
            
            uiItems[slotID].SetSlotID(slotID);
            uiItems[slotID].UpdateIcon(icon);
            uiItems[slotID].UpdateAmount(amount);
            uiItems[slotID].UpdateCooldownBar(currentCooldown, maxCooldown);
            uiItems[slotID].UpdateReadiness(isReady);
        }

      
        public void UpdateItemCooldown(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiItems[slotID].UpdateCooldownBar(current, max);
        }

        public void UpdateAmount(int? slotID, int amount)
        {
            if(slotID == null) return;
            uiItems[slotID].UpdateAmount(amount);
        }

        public void UpdateReadiness(int? slotID, bool isReady)
        {
            if(slotID == null) return;
            uiItems[slotID].UpdateReadiness(isReady);
        }

        public void RemoveItem(int? slotID)
        {
            if(slotID == null) return;
            uiItems[slotID].Clear();
        }
    }
}