using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap.Tower.Dragon
{
    [CreateAssetMenu(fileName = "SO_Dragon", menuName = "SO/Gameplay/Unit/Tower/Dragon", order = 51)]
    public class SoDragon : SO_Tower
    {
        [field: SerializeField] public AnimationClip DefaultClip { get; set; }
    }
}