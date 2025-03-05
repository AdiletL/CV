using Gameplay.Factory.Character.Creep;
using Machine;
using Zenject;

namespace Unit.Character.Creep
{
    public class BeholderSwitchAttackState : CreepSwitchAttackState
    {
        [Inject] private DiContainer diContainer;
        
        private CreepStateFactory creepStateFactory;
        private BeholderDefaultAttackState beholderDefaultAttackState;
        
        public void SetCreepStateFactory(CreepStateFactory creepStateFactory) => this.creepStateFactory = creepStateFactory;


        public override void Initialize()
        {
            base.Initialize();

            RangeAttack = so_CharacterAttack.Range;
            
            InitializeDefaultAttackState();
        }

        private void InitializeDefaultAttackState()
        {
            if (!this.stateMachine.IsStateNotNull(typeof(BeholderDefaultAttackState)))
            {
                beholderDefaultAttackState = (BeholderDefaultAttackState)creepStateFactory.CreateState(typeof(BeholderDefaultAttackState));
                diContainer.Inject(beholderDefaultAttackState);
                beholderDefaultAttackState.Initialize();
                this.stateMachine.AddStates(beholderDefaultAttackState);
            }
        }

        public override void SetState()
        {
            base.SetState();
            
            InitializeDefaultAttackState();
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.stateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.stateMachine.SetStates(desiredStates: beholderDefaultAttackState.GetType());
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            
            InitializeDefaultAttackState();
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.stateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.stateMachine.ExitCategory(category, beholderDefaultAttackState.GetType());
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();

            InitializeDefaultAttackState();
            
            beholderDefaultAttackState.SetTarget(currentTarget);
            if (!this.stateMachine.IsActivateType(beholderDefaultAttackState.GetType()))
                this.stateMachine.ExitOtherStates(beholderDefaultAttackState.GetType());
        }
    }
    
    public class BeholderSwitchAttackStateBuilder : CreepSwitchAttackStateBuilder
    {
        public BeholderSwitchAttackStateBuilder() : base(new BeholderSwitchAttackState())
        {
        }

        public BeholderSwitchAttackStateBuilder SetCreepStateFactory(CreepStateFactory creepStateFactory)
        {
            if(switchState is BeholderSwitchAttackState beholderSwitchAttackState)
                beholderSwitchAttackState.SetCreepStateFactory(creepStateFactory);
            return this;
        }
    }
}