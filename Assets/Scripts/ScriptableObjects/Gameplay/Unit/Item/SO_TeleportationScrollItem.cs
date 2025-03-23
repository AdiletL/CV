using Gameplay.Unit.Item;
using ScriptableObjects.Unit.Portal;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_TeleportationScrollItem", menuName = "SO/Gameplay/Item/TeleportationScroll", order = 51)]
    public class SO_TeleportationScrollItem : SO_Item
    {
        public override string ItemName { get; protected set; } = nameof(TeleportationScrollItem);
        [field: SerializeField, Space, PreviewField] public Texture2D SelectTargetCursor {get; private set;}
        [field: SerializeField] public AssetReferenceT<GameObject> PortalObject { get; private set; }
        [field: SerializeField] public SO_Portal EndPortalID { get; private set; }
    }
}