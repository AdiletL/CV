using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap
{
    [CreateAssetMenu(fileName = "SO_Fall", menuName = "SO/Gameplay/Unit/Trap/Fall/Normal", order = 51)]
    public class SO_Fall : SO_Trap
    {
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
    }
}