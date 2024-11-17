using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerMove", menuName = "SO/Gameplay/Character/Player/Move", order = 51)]
    public class SO_PlayerMove : SO_CharacterMove
    {
        [field: SerializeField, Space(5), Header("AnimationClips")]
        public AnimationClip[] RunClip { get; private set; }

        [field: SerializeField] public float RunSpeed { get; private set; }

    }
}