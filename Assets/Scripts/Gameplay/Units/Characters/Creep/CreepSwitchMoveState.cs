using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepSwitchMoveState : CharacterSwitchMoveState
    {

        public virtual bool IsCanMovement()
        {
            throw new System.NotImplementedException();
        }

    }

    public class CreepSwitchSwitchMoveStateBuilder : CharacterSwitchMoveStateBuilder
    {
        public CreepSwitchSwitchMoveStateBuilder(CharacterSwitchMoveState instance) : base(instance)
        {
        }
    }
}