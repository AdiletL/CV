using Gameplay;
using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterAttack : SO_UnitAttack
    {
        [field: SerializeField, Space(15), Range(0, 5f)] public float Range { get; private set; }
        [field: SerializeField, Space(10)] public float RotationSpeed { get; private set; } = 1000;
        [field: SerializeField, Space] public float ConsumptionEnduranceRate { get; private set; }

        [field: SerializeField, Space(15)] public AnimationEventConfig[] DefaultAnimations { get; private set; }
    }
}