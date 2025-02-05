using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepIdleState : CharacterIdleState
    {
        protected GameObject currentTarget;

        public virtual void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
    }

    public class CreepIdleStateBuilder : CharacterIdleStateBuilder
    {
        public CreepIdleStateBuilder(CharacterIdleState instance) : base(instance)
        {
        }
        
    }
}