using Gameplay.Skill;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerSkills", menuName = "SO/Gameplay/Unit/Character/Player/Skills", order = 51)]
    public class SO_PlayerSkills : SO_CharacterSkills
    {
        [ShowIf("@SkillTypeID.HasFlag(SkillType.dash)")]
        public DashConfig DashConfig;
    }
}