using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_ItemContainer", menuName = "SO/Gameplay/Item/Container", order = 51)]
    public class SO_ItemContainer : ScriptableObject
    {
        [SerializeField] private SO_Item[] items;

        public SO_Item GetItemConfig(string itemName)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if(string.Equals(items[i].ItemName, itemName))
                    return items[i];
            }
            throw new System.Exception("Item not found");
        }
    }
}