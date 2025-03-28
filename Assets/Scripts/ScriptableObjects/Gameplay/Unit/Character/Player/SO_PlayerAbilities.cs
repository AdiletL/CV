using Gameplay.Effect;
using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerAbilities", menuName = "SO/Gameplay/Unit/Character/Player/Abilities", order = 51)]
    public class SO_PlayerAbilities : SO_CharacterSkills
    {
        [field: SerializeField] public EffectConfigData EffectConfigData { get; set; }
    }
}