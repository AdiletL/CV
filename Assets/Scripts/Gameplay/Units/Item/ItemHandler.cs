using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Unit.Item
{
    public class ItemHandler : MonoBehaviour, IHandler, ICriticalDamageProvider
    {
        public event Action OnUpdate;

        private Dictionary<string, List<Item>> currentItems;
        

        public bool IsNullItem(string abilityType)
        {
            return currentItems == null || !currentItems.ContainsKey(abilityType);
        }
        
        public Item GetItem(string itemName, int? inventorySlotID)
        {
            if (currentItems == null || !currentItems.ContainsKey(itemName)) return null;
            
            for (int i = currentItems[itemName].Count - 1; i >= 0; i--)
            {
                if (currentItems[itemName][i].InventorySlotID == inventorySlotID)
                    return currentItems[itemName][i];
            }

            return null;
        }
        
        public List<Item> GetItems(string itemName)
        {
            if (currentItems == null || !currentItems.ContainsKey(itemName) ||
                currentItems[itemName].Count == 0) return null;
            return currentItems?[itemName];
        }
        
        public List<float> GetCriticalDamages(float baseDamage)
        {
            if (currentItems == null) return null;
            
            var list = new List<float>();
            foreach (var lists in currentItems.Values)
            {
                foreach (var VARIABLE in lists)
                {
                    if (VARIABLE is ICriticalDamageApplier criticalDamageApplier)
                    {
                        if(!criticalDamageApplier.CriticalDamage.TryApply()) continue;
                        var result = criticalDamageApplier.CriticalDamage.GetCalculateDamage(baseDamage);
                        list.Add(result);
                    }
                }
            }
            return list;
        }

        public void Initialize()
        {

        }

        private void Update() => OnUpdate?.Invoke();
        
        public void AddItem(Item item)
        {
            currentItems ??= new();
            
            if (IsNullItem(item.ItemName))
                currentItems.Add(item.ItemName, new List<Item>());
            
            OnUpdate += item.Update;
            
            currentItems[item.ItemName].Add(item);
        }

        public void RemoveItemByID(string itemName, int? inventorySlotID)
        {
            if (!IsNullItem(itemName))
            {
                for (int i = currentItems[itemName].Count - 1; i >= 0; i--)
                {
                    if (currentItems[itemName][i].InventorySlotID != inventorySlotID) continue;
                    
                    OnUpdate -= currentItems[itemName][i].Update;
                    currentItems[itemName][i].Exit();
                    
                    currentItems[itemName].Remove(currentItems[itemName][i]);
                    break;
                }
            }
        }
    }
}