using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class BeholderAttackState : CreepAttackState
    {
    }
    
    public class BeholderAttackStateBuilder : CreepAttackStateBuilder
    {
        public BeholderAttackStateBuilder() : base(new BeholderAttackState())
        {
        }
    }
}