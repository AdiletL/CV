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

        public void AddItem(ItemData data)
        {
            if(data.SlotID == null) return;
            
            uiItems[data.SlotID].SetSlotID(data.SlotID);
            uiItems[data.SlotID].SetIcon(data.Icon);
            uiItems[data.SlotID].SetValue(data.Amount);
            uiItems[data.SlotID].UpdateCooldownBar(0, 1);
            uiItems[data.SlotID].ChangeReadiness(false);
            foreach (var VARIABLE in data.AbilityConfigs)
            {
                if (VARIABLE.AbilityType != AbilityType.Nothing &&
                    (VARIABLE.AbilityBehaviour & AbilityBehaviour.Passive) == 0)
                {
                     uiItems[data.SlotID].ChangeReadiness(true);
                    return;
                }
            }
        }

        public void UpdateItem(ItemData data)
        {
            if(data.SlotID == null) return;
            
            uiItems[data.SlotID].SetSlotID(data.SlotID);
            uiItems[data.SlotID].SetIcon(data.Icon);
            uiItems[data.SlotID].SetValue(data.Amount);
        }

        public void UpdateItemCooldown(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiItems[slotID].UpdateCooldownBar(current, max);
        }

        public void ChangeReadiness(int? slotID, bool isReady)
        {
            if(slotID == null) return;
            uiItems[slotID].ChangeReadiness(isReady);
        }

        public void RemoveItem(int? slotID)
        {
            if(slotID == null) return;
            uiItems[slotID].Clear();
        }
    }
}