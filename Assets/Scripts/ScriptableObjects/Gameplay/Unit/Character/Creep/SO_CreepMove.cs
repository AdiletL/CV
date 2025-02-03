using UnityEngine;

namespace ScriptableObjects.Unit.Character.Creep
{
    public abstract class SO_CreepMove : SO_CharacterMove
    {
        [field: SerializeField, Space(5), Header("AnimationClips")]
        public AnimationClip[] RunClips { get; private set; }

        [field: SerializeField] public float RunSpeed { get; private set; }

        [field: SerializeField] public AnimationClip[] WalkClips { get; private set; }
        [field: SerializeField] public float WalkSpeed { get; private set; }
    }
}