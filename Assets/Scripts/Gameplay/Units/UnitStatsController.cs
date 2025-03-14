using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Unit
{
    public abstract class UnitStatsController : MonoBehaviour, IStatsController
    {
        [SerializeField] protected UnitController unitController;
        
        public Dictionary<StatType, Stat> Stats { get; protected set; }
        
        public Stat GetStat(StatType statType)
        {
            return Stats.GetValueOrDefault(statType);
        }

        public virtual void Initialize()
        {
            Stats ??= new Dictionary<StatType, Stat>();
        }
        
        protected void AddStatToDictionary(StatType statType, Stat stat)
        {
            Stats[statType] = stat;
        }
    }
}