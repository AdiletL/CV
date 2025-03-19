using System;
using Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay.Equipment
{
    public abstract class SO_Equipment : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject EquipmentPrefab { get; set; }
        [field: SerializeField] public StatConfig[] StatConfigs { get; protected set; }
    }
}