﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Ability
{
    [CreateAssetMenu(fileName = "SO_BaseAbility_", menuName = "SO/Gameplay/Ability/Base", order = 51)]
    public class SO_BaseAbilityConfig : ScriptableObject
    {
        [field: SerializeField, PreviewField] public Sprite Icon { get; private set; }
        [field: SerializeField] public AbilityType AbilityTypeID { get; private set; }
        [field: SerializeField] public AbilityBehaviour AbilityBehaviour { get; private set; }
    }
}