using UnityEngine;

namespace Unit.Character.Creep
{
    public class HedgehogWalkState : CreepWalkState
    {
        
    }

    public class HedgehogWalkStateBuilder : CreepWalkStateBuilder
    {
        public HedgehogWalkStateBuilder() : base(new HedgehogWalkState())
        {
        }
    }
}