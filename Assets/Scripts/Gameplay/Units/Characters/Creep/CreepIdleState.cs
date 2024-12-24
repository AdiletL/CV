using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepIdleState : CharacterIdleState
    {
       
    }

    public class CreepIdleStateBuilder : CharacterIdleStateBuilder
    {
        public CreepIdleStateBuilder(CharacterIdleState instance) : base(instance)
        {
        }
        
    }
}