using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Unit
{
    public abstract class UnitStatsController : MonoBehaviour, IStatsController<UnitStatType>
    {
        [SerializeField] protected UnitController unitController;
        
        public Dictionary<UnitStatType, Stat> Stats { get; protected set; }
        
        public Stat GetStat(UnitStatType abilityStatType)
        {
            return Stats.GetValueOrDefault(abilityStatType);
        }

        public virtual void Initialize()
        {
            Stats ??= new Dictionary<UnitStatType, Stat>();
        }
        
        protected void AddStatToDictionary(UnitStatType unitStatType, Stat stat)
        {
            Stats[unitStatType] = stat;
        }
    }

    [System.Serializable]
    public class UnitStatConfig : StatConfig
    {
        public UnitStatType UnitStatTypeID;
    }
    
    [System.Serializable]
    public class UnitStatConfigData
    {
        public UnitStatConfig[] StatConfigs;
    }
}