using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_MadnessMask", menuName = "SO/Gameplay/Item/MadnessMask", order = 51)]
    public class SO_MadnessMask : SO_Item
    {
        public override ItemName ItemNameID { get; protected set; } = ItemName.MadnessMask;
    }
}