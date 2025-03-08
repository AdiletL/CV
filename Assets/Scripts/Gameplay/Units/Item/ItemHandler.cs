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
        private Dictionary<ItemName, List<Item>> currentItems;


        public bool IsItemActive(ItemName itemNameID)
        {
            return currentItems.ContainsKey(itemNameID);
        }

        public bool IsNullItem(ItemName abilityType)
        {
            return currentItems == null || !currentItems.ContainsKey(abilityType);
        }

        public bool IsAbilityNull(AbilityType abilityType)
        {
            return currentAbilities == null || !currentAbilities.ContainsKey(abilityType);
        }
        
        public Item GetItem(ItemName itemNameID, int? inventorySlotID)
        {
            if (currentItems == null || !currentItems.ContainsKey(itemNameID)) return null;
            
            for (int i = currentItems[itemNameID].Count - 1; i >= 0; i--)
            {
                if (currentItems[itemNameID][i].InventorySlotID == inventorySlotID)
                    return currentItems[itemNameID][i];
            }

            return null;
        }
        
        public List<Item> GetItems(ItemName itemNameID)
        {
            return currentItems?[itemNameID];
        }

        public List<Ability.Ability> GetAbilities(AbilityType abilityType)
        {
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
            
            if (IsNullItem(item.ItemNameID))
                currentItems.Add(item.ItemNameID, new List<Item>());
            
            OnUpdate += item.Update;
            OnLateUpdate += item.LateUpdate;
            
            currentItems[item.ItemNameID].Add(item);
            AddAbilities(item.Abilities, item.InventorySlotID);
        }

        public void RemoveItemByID(ItemName itemNameID, int? inventorySlotID)
        {
            if (!IsNullItem(itemNameID))
            {
                for (int i = currentItems[itemNameID].Count - 1; i >= 0; i--)
                {
                    if (currentItems[itemNameID][i].InventorySlotID != inventorySlotID) continue;
                    
                    OnUpdate -= currentItems[itemNameID][i].Update;
                    OnLateUpdate -= currentItems[itemNameID][i].LateUpdate;
                    currentItems[itemNameID][i].Exit();
                    
                    RemoveAbilitiesByID(currentItems[itemNameID][i].Abilities, inventorySlotID);
                    currentItems[itemNameID].Remove(currentItems[itemNameID][i]);
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
                if(!currentAbilities.ContainsKey(abilities[i].AbilityType))
                    currentAbilities.Add(abilities[i].AbilityType, new List<Ability.Ability>());
                currentAbilities[abilities[i].AbilityType].Add(abilities[i]);
            }
        }

        public void RemoveAbilitiesByID(List<Ability.Ability> abilities, int? inventorySlotID)
        {
            if(abilities == null) return;
            
            for (int i = abilities.Count - 1; i >= 0; i--)
            {
                for (int j = currentAbilities[abilities[i].AbilityType].Count - 1; j >= 0; j--)
                {
                    if(currentAbilities[abilities[i].AbilityType][j].InventorySlotID == inventorySlotID)
                        currentAbilities[abilities[i].AbilityType].RemoveAt(j);
                }
            }
        }
    }
}