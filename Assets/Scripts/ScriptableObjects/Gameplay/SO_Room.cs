using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_Room_", menuName = "SO/Gameplay/Level/Room")]
    public class SO_Room : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField, Space] public AssetReferenceT<GameObject>[] RoomObjects { get; private set; }
    }
}