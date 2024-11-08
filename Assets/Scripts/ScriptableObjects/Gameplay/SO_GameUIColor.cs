using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameUIColor", menuName = "SO/Gameplay/UI Color", order = 51)]
    public class SO_GameUIColor : ScriptableObject
    {
        [field: SerializeField] public Gradient healthBarGradient { get; private set; }
    }
}