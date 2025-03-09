using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class BeholderIdleState : CreepIdleState
    { 

    }

    public class BeholderIdleStateBuilder : CreepIdleStateBuilder
    {
        public BeholderIdleStateBuilder() : base(new BeholderIdleState())
        {
        }
    }
}