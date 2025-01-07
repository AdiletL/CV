using UnityEngine;

namespace Unit
{
    public abstract class UnitCollision : MonoBehaviour
    {
        [field: SerializeField] public UnitController UnitController { get; protected set; }
    }
}