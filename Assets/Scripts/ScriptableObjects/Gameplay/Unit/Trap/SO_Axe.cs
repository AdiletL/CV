using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap
{
    [CreateAssetMenu(fileName = "SO_Axe", menuName = "SO/Gameplay/Unit/Trap/Axe", order = 51)]
    public class SO_Axe : SO_Trap
    {
        [field: SerializeField] public float SpeedPlayClip { get; private set; }
        [field: SerializeField] public AnimationClip PlayClip { get; private set; }
    }
}