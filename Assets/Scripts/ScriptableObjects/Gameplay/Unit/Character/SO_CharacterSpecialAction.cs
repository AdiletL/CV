﻿using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterSpecialAction : ScriptableObject
    {
        [field: SerializeField] public AbilityType[] AbilityTypesID { get; protected set; }
    }
}