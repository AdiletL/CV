using UnityEngine;

public abstract class SO_CharacterMove : ScriptableObject
{
    [field: SerializeField] public float RunSpeed { get; private set; }
    [field: SerializeField, Space, Header("AnimationClips")] public AnimationClip IdleClip { get; private set; }

}
