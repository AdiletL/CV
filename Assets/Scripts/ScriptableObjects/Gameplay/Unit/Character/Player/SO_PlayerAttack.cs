using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerAttack", menuName = "SO/Gameplay/Unit/Character/Player/Attack", order = 51)]
    public class SO_PlayerAttack: SO_CharacterAttack
    {
        [field: SerializeField, Space(15)] public AnimationEventConfig[] SwordAnimations { get; private set; }
        [field: SerializeField, Space(15)] public AnimationEventConfig[] BowAnimations { get; private set; }
        [field: SerializeField, Space] public InputType BlockInputType { get; private set; }
    }
}