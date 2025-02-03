using Machine;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderSwitchMoveState : CreepSwitchMoveState
    {
        private BeholderRunState beholderRunState;
        private BeholderPatrolState beholderPatrolState;
        

        private void InitializeRunState()
        {
            if(!this.stateMachine.IsStateNotNull(typeof(BeholderRunState)))
            {
                beholderRunState = (BeholderRunState)creepStateFactory.CreateState(typeof(BeholderRunState));
                beholderRunState.Initialize();
                this.stateMachine.AddStates(beholderRunState);
            }
        }

        private void InitializePatrolState()
        {
            if (!this.stateMachine.IsStateNotNull(typeof(BeholderPatrolState)))
            {
                beholderPatrolState = (BeholderPatrolState)creepStateFactory.CreateState(typeof(BeholderPatrolState));
                beholderPatrolState.Initialize();
                this.stateMachine.AddStates(beholderPatrolState);
            }
        }

        public override void SetState()
        {
            base.SetState();
            if (currentTarget)
            {
                InitializeRunState();
                
                beholderRunState.SetTarget(currentTarget);
                if(!this.stateMachine.IsActivateType(beholderRunState.GetType()))
                    this.stateMachine.SetStates(desiredStates: beholderRunState.GetType());
            }
            else
            {
                InitializePatrolState();
                
                if(!this.stateMachine.IsActivateType(beholderPatrolState.GetType()))
                    this.stateMachine.SetStates(desiredStates: beholderPatrolState.GetType());
            }
            currentTarget = null;
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            if (currentTarget)
            {
                InitializeRunState();
                
                beholderRunState.SetTarget(currentTarget);
                if(!this.stateMachine.IsActivateType(beholderRunState.GetType()))
                    this.stateMachine.ExitCategory(category, beholderRunState.GetType());
            }
            else 
            {
                InitializePatrolState();
                
                if(!this.stateMachine.IsActivateType(beholderPatrolState.GetType()))
                    this.stateMachine.ExitCategory(category, beholderPatrolState.GetType());
            }
            currentTarget = null;
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();
            if (currentTarget)
            {
                InitializeRunState();
                
                beholderRunState.SetTarget(currentTarget);
                if(!this.stateMachine.IsActivateType(beholderRunState.GetType()))
                    this.stateMachine.ExitOtherStates(beholderRunState.GetType());
            }
            else 
            {
                InitializePatrolState();
                
                if(!this.stateMachine.IsActivateType(beholderPatrolState.GetType()))
                    this.stateMachine.ExitOtherStates(beholderPatrolState.GetType());
            }
            currentTarget = null;
        }
    }
    
    public class BeholderSwitchSwitchMoveStateBuilder : CreepSwitchSwitchMoveStateBuilder
    {
        public BeholderSwitchSwitchMoveStateBuilder() : base(new BeholderSwitchMoveState())
        {
        }
    }
}