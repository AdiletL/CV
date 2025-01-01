using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterMove : ScriptableObject
    {
        [field: SerializeField, Space, Header("Idle")]
        public AnimationClip[] IdleClip { get; private set; }

        [field: SerializeField, Space(10), Header("Rotate")] public float RotateSpeed { get; private set; } = 800;
        
        [field: SerializeField, Space(10), Header("Jump")] public AnimationCurve JumpCurve { get; private set; }
        [field: SerializeField] public AnimationClip JumpClip { get; private set; }
        [field: SerializeField] public float JumpDuration { get; private set; } = 1f;
        [field: SerializeField] public float JumpHeight { get; private set; } = 1.5f;
        [field: SerializeField] public int MaxJumpCount { get; private set; } = 1;
    }
}
