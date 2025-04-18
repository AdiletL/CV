﻿using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerSkillInventory", menuName = "SO/Gameplay/Unit/Character/Player/Inventory/Skill")]
    public class SO_PlayerAbilityInventory : ScriptableObject
    {
        [field: SerializeField] public InputType SelectItemBlockInputType { get; private set; } = InputType.Attack;
        [field: SerializeField] public int MaxSlot { get; private set; } = 1;
    }
}