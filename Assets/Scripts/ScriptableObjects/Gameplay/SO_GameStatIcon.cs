using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "SO_GameStatIcon", menuName = "SO/Gameplay/Stat Icon")]
    public class SO_GameStatIcon : ScriptableObject
    {
        [field: SerializeField, PreviewField] public Sprite Damage { get; private set; }
        [field: SerializeField, PreviewField] public Sprite MovementSpeed{ get; private set; }
        [field: SerializeField, PreviewField] public Sprite Armor{ get; private set; }
        [field: SerializeField, PreviewField] public Sprite RangeAttack { get; private set; }
    }
}