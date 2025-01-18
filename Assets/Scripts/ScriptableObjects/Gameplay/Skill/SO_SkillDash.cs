using UnityEngine;

namespace ScriptableObjects.Gameplay.Skill
{
    [CreateAssetMenu(fileName = "SO_SkillDash", menuName = "SO/Gameplay/Skill/Dash", order = 51)]
    public class SO_SkillDash : SO_Skill
    {
        [field: SerializeField, Space] public float DashSpeed { get; private set; } = 10;
        [field: SerializeField, Space] public float DashDuration { get; private set; } = .15f;
    }
}