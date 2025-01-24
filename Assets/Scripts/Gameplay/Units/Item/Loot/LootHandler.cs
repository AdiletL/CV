using System;
using System.Collections.Generic;
using Unit.Item.Loot;
using UnityEngine;

namespace Gameplay.Loot
{
    public class LootHandler : MonoBehaviour, IHandler
    {
        private Dictionary<string, LootData> loots = new();

        public bool TryGetLoot(string name)
        {
            if (loots.ContainsKey(name))
                return true;

            return false;
        }

        public LootData GetLoot(string name)
        {
            return loots[name];
        }
        
        
        public void Initialize()
        {
            
        }

        public void AddLoot(LootData data)
        {
            if (!loots.ContainsKey(data.Name))
                loots.Add(data.Name, data);
                
            loots[data.Name].Amount += data.Amount;
        }

        public void RemoveLoot(LootData data)
        {
            if(!loots.ContainsKey(data.Name)) return;
            
            loots[data.Name].Amount -= data.Amount;
            if(loots[data.Name].Amount <= 0)
                loots.Remove(data.Name);
        }
    }
}