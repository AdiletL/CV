using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_Room_", menuName = "SO/Gameplay/Level/Room")]
    public class SO_Room : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField, Space] public AssetReferenceGameObject[] RoomObjects { get; private set; }
        [field: SerializeField] public SO_Room[] NextRooms { get; private set; }
    }
}