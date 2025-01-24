using UnityEngine;

namespace ScriptableObjects.Unit.Item.Container
{
    public abstract class SO_Container : SO_Item
    {
        [field: SerializeField] public AnimationClip OpenClip { get; protected set; }
        [field: SerializeField] public AnimationClip CloseClip { get; protected set; }
    }
}