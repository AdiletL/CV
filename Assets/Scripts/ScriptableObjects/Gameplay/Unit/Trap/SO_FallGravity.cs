using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap
{
    [CreateAssetMenu(fileName = "SO_FallGravity", menuName = "SO/Gameplay/Unit/Trap/Fall/Gravity", order = 51)]
    public class SO_FallGravity : SO_Trap
    {
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float Mass { get; private set; }
    }
}