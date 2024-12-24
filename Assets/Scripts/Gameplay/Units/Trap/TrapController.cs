using ScriptableObjects.Gameplay.Trap;
using Unit;
using UnityEngine;

namespace Unit.Trap
{
    public abstract class TrapController : UnitController, ITrap
    {
        public override UnitType UnitType { get; } = UnitType.trap;

        [SerializeField] protected SO_Trap so_Trap;

        public abstract void Activate();
        public abstract void Deactivate();

    }
}