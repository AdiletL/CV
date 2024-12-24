using UnityEngine;

namespace ScriptableObjects.Gameplay.Trap.Tower.Dragon
{
    [CreateAssetMenu(fileName = "SO_DragonAttack", menuName = "SO/Gameplay/Tower/Dragon", order = 51)]
    public class SO_DragonAttack : SO_TowerAttack
    {
        [field: SerializeField] public AnimationClip DefaultClip { get; set; }
    }
}