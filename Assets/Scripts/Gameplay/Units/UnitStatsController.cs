using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
    public abstract class UnitStatsController : MonoBehaviour
    {
        [SerializeField] protected UnitController unitController;
        
        private Dictionary<StatType, List<Stat>> statsKey;
        
        public List<Stat> GetStat(StatType statType)
        {
            return statsKey.GetValueOrDefault(statType);
        }

        public virtual void Initialize()
        {
            statsKey ??= new Dictionary<StatType, List<Stat>>();
        }
        

        protected void AddStatToDictionary(StatType statType, Stat stat)
        {
            statsKey.TryAdd(statType, new List<Stat>());
            statsKey[statType].Add(stat);
        }
    }
}