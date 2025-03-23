using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Unit.Item
{
    public class ItemHandler : MonoBehaviour, IHandler
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;

        private Dictionary<AbilityType, List<Ability.Ability>> currentAbilities;
        private Dictionary<string, List<Item>> currentItems;


        public bool IsItemActive(string itemName)
        {
            return currentItems.ContainsKey(itemName);
        }

        public bool IsNullItem(string abilityType)
        {
            return currentItems == null || !currentItems.ContainsKey(abilityType);
        }

        public bool IsAbilityNull(AbilityType abilityType)
        {
            return currentAbilities == null || !currentAbilities.ContainsKey(abilityType);
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

        public List<Ability.Ability> GetAbilities(AbilityType abilityType)
        {
            if(currentAbilities == null || !currentAbilities.ContainsKey(abilityType) ||
               currentAbilities[abilityType].Count == 0) return null;
            return currentAbilities?[abilityType];
        }

        public void Initialize()
        {

        }

        private void Update() => OnUpdate?.Invoke();
        private void LateUpdate() => OnLateUpdate?.Invoke();
        
        
        public void AddItem(Item item)
        {
            currentItems ??= new();
            
            if (IsNullItem(item.ItemName))
                currentItems.Add(item.ItemName, new List<Item>());
            
            OnUpdate += item.Update;
            OnLateUpdate += item.LateUpdate;
            
            currentItems[item.ItemName].Add(item);
            AddAbilities(item.Abilities, item.InventorySlotID);
        }

        public void RemoveItemByID(string itemName, int? inventorySlotID)
        {
            if (!IsNullItem(itemName))
            {
                for (int i = currentItems[itemName].Count - 1; i >= 0; i--)
                {
                    if (currentItems[itemName][i].InventorySlotID != inventorySlotID) continue;
                    
                    OnUpdate -= currentItems[itemName][i].Update;
                    OnLateUpdate -= currentItems[itemName][i].LateUpdate;
                    currentItems[itemName][i].Exit();
                    
                    RemoveAbilitiesByID(currentItems[itemName][i].Abilities, inventorySlotID);
                    currentItems[itemName].Remove(currentItems[itemName][i]);
                    break;
                }
            }
        }

        public void AddAbilities(List<Ability.Ability> abilities, int? inventorySlotID)
        {
            if(abilities == null) return;
            currentAbilities ??= new ();
            
            for (int i = 0; i < abilities.Count; i++)
            {
                abilities[i].SetInventorySlotID(inventorySlotID);
                if(!currentAbilities.ContainsKey(abilities[i].AbilityTypeID))
                    currentAbilities.Add(abilities[i].AbilityTypeID, new List<Ability.Ability>());
                currentAbilities[abilities[i].AbilityTypeID].Add(abilities[i]);
            }
        }

        public void RemoveAbilitiesByID(List<Ability.Ability> abilities, int? inventorySlotID)
        {
            if(abilities == null) return;
            
            for (int i = abilities.Count - 1; i >= 0; i--)
            {
                for (int j = currentAbilities[abilities[i].AbilityTypeID].Count - 1; j >= 0; j--)
                {
                    if(currentAbilities[abilities[i].AbilityTypeID][j].InventorySlotID == inventorySlotID)
                        currentAbilities[abilities[i].AbilityTypeID].RemoveAt(j);
                }
                if(currentAbilities[abilities[i].AbilityTypeID].Count == 0)
                    currentAbilities.Remove(abilities[i].AbilityTypeID);
            }
        }
    }
}