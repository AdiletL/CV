using UnityEngine;

namespace ScriptableObjects.Unit.Character
{
    public abstract class SO_CharacterInformation : SO_UnitInformation
    {
        [field: SerializeField] public TextAsset Description { get; protected set; }
    }
}