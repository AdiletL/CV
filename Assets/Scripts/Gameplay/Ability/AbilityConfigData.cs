using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Ability
{
    [System.Serializable]
    public class AbilityConfigData
    {
        public List<AbilityType> AbilityTypeID;
        
        [ShowIf("@AbilityTypeID.Contains(AbilityType.Vampirism)"), Space]
        public VampirismConfig VampirismConfig;
        
        [ShowIf("@AbilityTypeID.Contains(AbilityType.BlockPhysicalDamage)"), Space]
        public BlockPhysicalDamageConfig BlockPhysicalDamageConfig;
        
        [ShowIf("@AbilityTypeID.Contains(AbilityType.Dash)"), Space]
        public DashConfig DashConfig;
    }
}