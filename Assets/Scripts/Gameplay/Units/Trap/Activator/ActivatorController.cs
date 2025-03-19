using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Unit.Trap
{
    public abstract class ActivatorController : TrapController
    {
        [SerializeField] private TrapController[] traps;
        
        public  void Trigger()
        {
            foreach (var VARIABLE in traps)
            {
                //VARIABLE.SetTarget(CurrentTarget);
                VARIABLE.Activate();
            }
        }

        public  void Reset()
        {
            foreach (var VARIABLE in traps)
            {
                VARIABLE.Deactivate();
            }
        }
    }
}