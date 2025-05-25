using Gameplay;
using Gameplay.Ability;
using Gameplay.AttackModifier;
using Gameplay.Effect;
using Gameplay.Unit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Ability
{
    public abstract class SO_Ability : ScriptableObject
    {
        public abstract AbilityType AbilityTypeID { get; }
        [field: SerializeField, PreviewField] public Sprite Icon { get; private set; }
        [field: SerializeField] public AbilityBehaviour AbilityBehaviour { get; private set; }
        [field: SerializeField, Space(10)] public DescriptionConfig DescriptionConfig { get; private set; }
        
        [field: SerializeField, Space(10)] public int MaxLevelScale { get; private set; }
        [field: SerializeField, Space(25)] public AbilityStatConfigData AbilityStatConfigData { get; private set; }
        
        [field: SerializeField, Space(25)] public AttackModifierConfigData AttackModifierConfigData { get; private set; }
        
        [field: SerializeField, Space(25)] public UnitStatConfigData UnitStatConfigData { get; private set; }
        
        [field: SerializeField, Space(25)] public EffectConfigData EffectConfigData { get; private set; }
        
        [field: SerializeField] public float TimerCast { get; private set; }
    }
}