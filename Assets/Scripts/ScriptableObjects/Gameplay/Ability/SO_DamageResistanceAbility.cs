using Gameplay;
using Gameplay.Unit;
using UnityEngine;

namespace ScriptableObjects.Ability
{
    [CreateAssetMenu(fileName = "SO_DamageResistance", menuName = "SO/Gameplay/Ability/Damage Resistance", order = 51)]
    public class SO_DamageResistanceAbility : SO_Ability
    {
        public override AbilityType AbilityTypeID { get; } = AbilityType.DamageResistance;
        
        [field: SerializeField, Space(15)] public UnitStatConfig[] StatConfigs { get; private set; }
        [field: SerializeField] public AnimationClip Clip { get; private set; }
    }
}