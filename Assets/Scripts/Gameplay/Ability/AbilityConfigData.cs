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
        
        [ShowIf("@AbilityTypeID.Contains(AbilityType.DamageResistance)"), Space]
        public DamageResistanceConfig damageResistanceConfig;
        
        [ShowIf("@AbilityTypeID.Contains(AbilityType.Dash)"), Space]
        public DashConfig DashConfig;
    }
}