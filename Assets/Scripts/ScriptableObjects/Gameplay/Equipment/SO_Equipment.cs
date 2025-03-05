using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay.Equipment
{
    public abstract class SO_Equipment : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject EquipmentPrefab { get; set; }
        [field: SerializeField] public float ReduceEndurance { get; protected set; }
    }
}