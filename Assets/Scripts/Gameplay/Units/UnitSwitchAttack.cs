using Machine;
using ScriptableObjects.Unit;

namespace Unit
{
    public abstract class UnitSwitchAttack
    {
        public SO_UnitAttack SO_UnitAttack { get; set; }
    }

    public abstract class UnitSwitchAttackStateBuilder<T> where T : UnitSwitchAttack
    {
        protected UnitSwitchAttack state;
        public UnitSwitchAttackStateBuilder(UnitSwitchAttack instance)
        {
            state = instance;
        }

        public UnitSwitchAttackStateBuilder<T> SetConfig(SO_UnitAttack config)
        {
            state.SO_UnitAttack = config;
            return this;
        }

        public UnitSwitchAttack Build()
        {
            return state;
        }
    }
}