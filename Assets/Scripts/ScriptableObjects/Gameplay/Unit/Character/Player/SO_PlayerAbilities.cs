using Gameplay.Ability;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerAbilities", menuName = "SO/Gameplay/Unit/Character/Player/Abilities", order = 51)]
    public class SO_PlayerAbilities : SO_CharacterSkills
    {
        [ShowIf("@AbilityTypeID.HasFlag(AbilityType.Dash)"), Space]
        public DashConfig DashConfig;
        
    }
}