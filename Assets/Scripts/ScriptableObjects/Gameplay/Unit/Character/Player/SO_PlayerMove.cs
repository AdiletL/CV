using Movement;
using Unit.Character.Player;
using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerMove", menuName = "SO/Gameplay/Unit/Character/Player/Move", order = 51)]
    public class SO_PlayerMove : SO_CharacterMove
    {
        [field: SerializeField, Space(5), Header("Run")]
        public AnimationClip[] RunClip { get; private set; }

        [field: SerializeField] public float RunSpeed { get; private set; }
        [field: SerializeField] public float BaseRunReductionEndurance { get; private set; }
        
        [field: SerializeField, Space(10), Header("Jump")] public JumpInfo JumpInfo { get; private set; }
        
        [field: SerializeField, Space] public InputType BlockInputType { get; private set; }
    }
}