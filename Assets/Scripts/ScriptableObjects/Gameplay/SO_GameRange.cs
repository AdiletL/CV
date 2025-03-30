using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameRange", menuName = "SO/Gameplay/Range", order = 51)]
    public class SO_GameRange : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceT<GameObject> AttackPrefab { get; private set; }
        [field: SerializeField] public AssetReferenceT<GameObject> CastPrefab { get; private set; }
    }
}