using UnityEngine;

namespace ScriptableObjects.Unit.Character.Creep
{
    public abstract class SO_CreepMove : SO_CharacterMove
    {
        [field: SerializeField] public AnimationClip[] WalkClips { get; private set; }
        [field: SerializeField] public float WalkSpeed { get; private set; }
        
        [field: SerializeField, Space] public float TimerRunToTarget { get; private set; } 
    }
}