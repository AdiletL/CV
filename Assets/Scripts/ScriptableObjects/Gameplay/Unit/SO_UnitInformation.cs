using Unit;
using UnityEngine;

namespace ScriptableObjects.Unit
{
    public abstract class SO_UnitInformation : ScriptableObject
    {
        [field: SerializeField] public Sprite Icon { get; protected set; }
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField]  public EntityType EntityType { get; protected set; }
    }
}