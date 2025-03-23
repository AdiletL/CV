using Gameplay.Unit.Item;
using UnityEngine;

namespace ScriptableObjects.Unit.Item
{
    [CreateAssetMenu(fileName = "SO_MadnessMaskItem", menuName = "SO/Gameplay/Item/MadnessMask", order = 51)]
    public class SO_MadnessMaskItem : SO_Item
    {
        public override string ItemName { get; protected set; } = nameof(MadnessMaskItem);
    }
}