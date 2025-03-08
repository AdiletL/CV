using UnityEngine;

namespace Gameplay.Unit
{
    public class UnitCenter : MonoBehaviour
    {
        [field: SerializeField] public Transform Center { get; private set; }
    }
}