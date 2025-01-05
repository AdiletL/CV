using System.Collections.Generic;
using UnityEngine;

namespace Unit.Trap
{
    public abstract class ActivatorController : TrapController
    {
        [SerializeField] private TrapController[] traps;

        //Test
                
        private List<TrapController> testTraps = new List<TrapController>();
        public void AddTrap(TrapController trap)
        {
            testTraps.Add(trap);
        }

        public override void Activate()
        {
            foreach (var VARIABLE in testTraps)
            {
                VARIABLE.Activate();
            }
        }

        public override void Deactivate()
        {
            foreach (var VARIABLE in testTraps)
            {
                VARIABLE.Deactivate();
            }
        }
    }
}