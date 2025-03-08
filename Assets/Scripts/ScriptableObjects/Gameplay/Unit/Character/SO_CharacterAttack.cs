using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterAttack : SO_UnitAttack
    {
        [field: SerializeField, Space(15), Range(0, 5f)] public float Range { get; private set; }

        [field: SerializeField, Header("Percent"), Range(0, 1)]
        public float ApplyDamageMoment { get; private set; } = .55f;
        [field: SerializeField, Space, Range(0, 1)] public float BaseReductionEndurance { get; private set; }
        
        [field: SerializeField, Space(10), Header("Default")] 
        public AnimationClip[] DefaultAttackClips { get; private set; }
        [field: SerializeField] public AnimationClip DefaultCooldownClip { get; private set; }
    }
}