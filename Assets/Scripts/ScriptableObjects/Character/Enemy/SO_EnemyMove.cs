using UnityEngine;

namespace ScriptableObjects.Character.Enemy
{
    public abstract class SO_EnemyMove : SO_CharacterMove
    {
        [field: SerializeField, Space(5), Header("AnimationClips")]
        public AnimationClip RunClip { get; private set; }

        [field: SerializeField] public float RunSpeed { get; private set; }

        [field: SerializeField] public AnimationClip WalkClip { get; private set; }
        [field: SerializeField] public float WalkSpeed { get; private set; }
    }
}