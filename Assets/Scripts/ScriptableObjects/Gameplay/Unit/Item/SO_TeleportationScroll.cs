using ScriptableObjects.Unit.Portal;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_TeleportationScroll", menuName = "SO/Gameplay/Item/TeleportationScroll", order = 51)]
    public class SO_TeleportationScroll : SO_Item
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.TeleportationScroll;
        [field: SerializeField, Space] public AssetReferenceT<GameObject> PortalObject { get; private set; }
        [field: SerializeField] public SO_Portal EndPortalID { get; private set; }
    }
}