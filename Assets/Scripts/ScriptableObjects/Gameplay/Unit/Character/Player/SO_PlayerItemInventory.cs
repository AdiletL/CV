using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerItemInventory", menuName = "SO/Gameplay/Unit/Character/Player/Inventory/Item", order = 51)]
    public class SO_PlayerItemInventory : ScriptableObject
    {
        [field: SerializeField] public InputType SelectItemBlockInputType { get; private set; } = InputType.Attack;
        [field: SerializeField] public int MaxCountItem { get; private set; } = 1;
        [field: SerializeField] public AssetReferenceT<GameObject> RangeCastPrefab { get; private set; }
    }
}