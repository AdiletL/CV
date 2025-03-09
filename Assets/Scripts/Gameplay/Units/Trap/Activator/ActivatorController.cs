using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public abstract class ActivatorController : TrapController
    {
        [SerializeField] private TrapController[] traps;
        
        public override void Trigger()
        {
            foreach (var VARIABLE in traps)
            {
                VARIABLE.SetTarget(CurrentTarget);
                VARIABLE.Activate();
            }
        }

        public override void Reset()
        {
            foreach (var VARIABLE in traps)
            {
                VARIABLE.Deactivate();
            }
        }
    }
}