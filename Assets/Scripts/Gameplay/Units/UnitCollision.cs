using UnityEngine;

namespace Gameplay.Unit
{
    public abstract class UnitCollision : MonoBehaviour
    {
        [field: SerializeField] public UnitController UnitController { get; protected set; }
    }
}