using Machine;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterSwitchState : UnitSwitchState
    {
        protected UnitAnimation unitAnimation;
        
        public void SetUnitAnimation(UnitAnimation unitAnimation) => this.unitAnimation = unitAnimation;
    }

    public abstract class CharacterSwitchStateBuilder : UnitSwitchStateBuilder<CharacterSwitchState>
    {
        protected CharacterSwitchStateBuilder(UnitSwitchState instance) : base(instance)
        {
        }

        public CharacterSwitchStateBuilder SetUnitAnimation(UnitAnimation unitAnimation)
        {
            if (switchState is CharacterSwitchState characterSwitchState)
            {
                characterSwitchState.SetUnitAnimation(unitAnimation);
            }
            return this;
        }
    }
}