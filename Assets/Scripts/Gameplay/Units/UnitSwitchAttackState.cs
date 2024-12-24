using Machine;
using ScriptableObjects.Unit;

namespace Unit
{
    public abstract class UnitSwitchAttackState : State
    {
        public SO_UnitAttack SO_UnitAttack { get; set; }
    }

    public abstract class UnitSwitchAttackStateBuilder : StateBuilder<UnitSwitchAttackState>
    {
        public UnitSwitchAttackStateBuilder(UnitSwitchAttackState instance) : base(instance)
        {
        }

        public UnitSwitchAttackStateBuilder SetConfig(SO_UnitAttack config)
        {
            state.SO_UnitAttack = config;
            return this;
        }
    }
}