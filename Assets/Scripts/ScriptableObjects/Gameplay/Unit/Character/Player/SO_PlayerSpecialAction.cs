using UnityEngine;

namespace ScriptableObjects.Unit.Character.Player
{
    [CreateAssetMenu(fileName = "SO_PlayerSpecialAction", menuName = "SO/Gameplay/Unit/Character/Player/SpecialAction", order = 51)]
    public class SO_PlayerSpecialAction : SO_CharacterSpecialAction
    {
        [field: SerializeField, Space] public InputType BlockInputType { get; private set; }
    }
}