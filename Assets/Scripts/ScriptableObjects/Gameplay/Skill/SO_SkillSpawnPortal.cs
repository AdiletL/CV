using ScriptableObjects.Unit.Portal;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay.Skill
{
    [CreateAssetMenu(fileName = "SO_SkillSpawnPortal", menuName = "SO/Gameplay/Skill/SpawnPortal", order = 51)]
    public class SO_SkillSpawnPortal : SO_Skill
    {
        [field: SerializeField, Space] public AssetReferenceT<GameObject> SpawnPortalPrefab { get; private set; }
        [field: SerializeField] public SO_Portal IDStartPortal { get; private set; }
    }
}