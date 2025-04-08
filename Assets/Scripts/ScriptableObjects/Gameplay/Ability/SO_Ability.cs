using Gameplay;
using Gameplay.AttackModifier;
using Gameplay.Effect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Ability
{
    public abstract class SO_Ability : ScriptableObject
    {
        public abstract AbilityType AbilityTypeID { get; }
        [field: SerializeField, PreviewField] public Sprite Icon { get; private set; }
        [field: SerializeField] public AbilityBehaviour AbilityBehaviour { get; private set; }
        
        [Space(25)]
        public AttackModifierConfigData AttackModifierConfigData;
        
        [Space(25)]
        public StatConfigData StatConfigData;
        
        [Space(25)]
        public EffectConfigData EffectConfigData;
        
        [field: SerializeField, Space(25)] public float Cooldown { get; private set; }
        [field: SerializeField] public float TimerCast { get; private set; }
        [field: SerializeField] public float Range { get; private set; }
    }
}