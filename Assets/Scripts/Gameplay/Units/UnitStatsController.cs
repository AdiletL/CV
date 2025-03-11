using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Unit
{
    public abstract class UnitStatsController : MonoBehaviour
    {
        [SerializeField] protected UnitController unitController;
        
        private Dictionary<StatType, Stat> statsKey;
        
        public Stat GetStat(StatType statType)
        {
            return statsKey.GetValueOrDefault(statType);
        }

        public virtual void Initialize()
        {
            statsKey ??= new Dictionary<StatType, Stat>();
        }
        
        protected void AddStatToDictionary(StatType statType, Stat stat)
        {
            statsKey[statType] = stat;
        }
    }
}