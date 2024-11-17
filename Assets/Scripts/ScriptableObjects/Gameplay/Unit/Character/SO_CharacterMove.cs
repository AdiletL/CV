using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterMove : ScriptableObject
    {
        [field: SerializeField, Space, Header("AnimationClips")]
        public AnimationClip[] IdleClip { get; private set; }

        [field: SerializeField] public float RotateSpeed { get; private set; } = 800;

    }
}
