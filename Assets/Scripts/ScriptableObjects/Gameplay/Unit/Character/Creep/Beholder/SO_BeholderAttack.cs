using UnityEngine;

namespace ScriptableObjects.Unit.Character.Creep
{
    [CreateAssetMenu(fileName = "SO_BeholderAttack", menuName = "SO/Gameplay/Unit/Character/Creep/Beholder/Attack", order = 51)]
    public class SO_BeholderAttack : SO_CreepAttack
    {
        [field: SerializeField, Space(15)] public AnimationClip[] AttackClips { get; set; }
        [field: SerializeField] public AnimationClip CooldownClip { get; set; }
    }
}