using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.AttackModifier
{
    public abstract class AttackModifier : IAttackModifier
    {
        public abstract AttackModifierType AttackModifierTypeID { get; }
    }

    [System.Serializable]
    public class AttackModifierConfigData
    {
        public AttackModifierType AttackModifierTypeID;
        
        [ShowIf("@AttackModifierTypeID.HasFlag(AttackModifierType.CriticalDamage)"), Space]
        public CriticalDamageConfig CriticalDamageConfig;
    }
}