using UnityEngine;

namespace ScriptableObjects.Unit.Item.Container
{
    [CreateAssetMenu(fileName = "SO_Chest", menuName = "SO/Gameplay/Unit/Container/Chest", order = 51)]
    public class SO_Chest : SO_Container
    {
        [field: SerializeField] public ItemConfigData[] so_Item { get; private set; }
    }
}