using System;
using Gameplay;
using Gameplay.Unit;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay.Equipment
{
    public abstract class SO_Equipment : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject EquipmentPrefab { get; set; }
        [field: SerializeField] public UnitStatConfig[] StatConfigs { get; protected set; }
        
    }
}