using UnityEngine;

namespace ScriptableObjects.Unit.InteractableObject.Container
{
    public abstract class SO_Container : SO_InteractableObject
    {
        [field: SerializeField] public AnimationClip OpenClip { get; protected set; }
        [field: SerializeField] public AnimationClip CloseClip { get; protected set; }
    }
}