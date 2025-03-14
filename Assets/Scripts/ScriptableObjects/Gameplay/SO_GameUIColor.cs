using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameUIColor", menuName = "SO/Gameplay/UI Color", order = 51)]
    public class SO_GameUIColor : ScriptableObject
    {
        [field: SerializeField] public Gradient HealthBarGradient { get; private set; }
        [field: SerializeField] public Gradient EnduranceBarGradient { get; private set; }
        [field: SerializeField] public Gradient ManaBarGradient { get; private set; }
    }
}