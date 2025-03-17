using Gameplay.Movement;
using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerMove", menuName = "SO/Gameplay/Unit/Character/Player/Move", order = 51)]
    public class SO_PlayerMove : SO_CharacterMove
    {
        [field: SerializeField] public float ConsumptionEnduranceRate { get; private set; }
        
        [field: SerializeField, Space(10), Header("Jump")] public JumpConfig JumpConfig { get; private set; }
        
        [field: SerializeField, Space] public InputType BlockInputType { get; private set; }
    }
}