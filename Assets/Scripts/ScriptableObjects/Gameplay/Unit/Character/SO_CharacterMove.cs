using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterMove : ScriptableObject
    {
        [field: SerializeField, Space, Header("Idle")]
        public AnimationClip[] IdleClip { get; private set; }

        [field: SerializeField, Space(5), Header("Run")]
        public AnimationClip[] RunClip { get; private set; }

        [field: SerializeField] public float RunSpeed { get; private set; }
        
        [field: SerializeField, Space(10), Header("Rotate")] public float RotateSpeed { get; private set; } = 800;
    }
}
