using UnityEngine;

namespace ScriptableObjects.Ability
{
    [CreateAssetMenu(fileName = "SO_Dash", menuName = "SO/Gameplay/Ability/Dash", order = 51)]
    public class SO_DashAbility : SO_Ability
    {
        public override AbilityType AbilityTypeID { get; } = AbilityType.Dash;
        
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
    }
}