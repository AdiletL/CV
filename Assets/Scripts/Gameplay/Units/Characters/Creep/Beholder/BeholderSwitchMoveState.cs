using Machine;
using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderSwitchMoveState : CreepSwitchMoveState
    {
        private SO_BeholderMove so_BeholderMove;
        private BeholderAnimation beholderAnimation;
        private BeholderRunState beholderRunState;
        private BeholderPatrolState beholderPatrolState;
        private ISwitchState beholderSwitchAttack;
        
        public CharacterController CharacterController { get; set; }
        public Transform Start { get; set; }
        public Transform End { get; set; }
        

        public override bool IsCanMovement()
        {
            return Start && End || currentTarget;
        }

        private BeholderPatrolState CreatePatrolState()
        {
            return (BeholderPatrolState)new BeholderPatrolStateBuilder()
                .SetCharacterController(CharacterController)
                .SetCenter(center)
                .SetEnemyAnimation(beholderAnimation)
                .SetWalkClip(so_BeholderMove.WalkClip)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetCharacterSwitchAttack(beholderSwitchAttack)
                .SetStart(Start)
                .SetEnd(End)
                .SetGameObject(gameObject)
                .SetMovementSpeed(so_BeholderMove.WalkSpeed)
                .SetStateMachine(stateMachine)
                .Build();
        }

        private BeholderRunState CreateRunState()
        {
            return (BeholderRunState)new BeholderRunStateBuilder()
                .SetCenter(center)
                .SetCharacterController(CharacterController)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetUnitAnimation(beholderAnimation)
                .SetRunClips(so_BeholderMove.RunClip)
                .SetGameObject(gameObject)
                .SetMovementSpeed(so_BeholderMove.RunSpeed)
                .SetStateMachine(stateMachine)
                .Build();
        }
        

        private void InitializeRunState()
        {
            if(!this.stateMachine.IsStateNotNull(typeof(BeholderRunState)))
            {
                beholderRunState = CreateRunState();
                beholderRunState.Initialize();
                this.stateMachine.AddStates(beholderRunState);
            }
        }

        private void InitializePatrolState()
        {
            if (!this.stateMachine.IsStateNotNull(typeof(BeholderPatrolState)))
            {
                beholderPatrolState = CreatePatrolState();
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
        
        public BeholderSwitchSwitchMoveStateBuilder SetStart(Transform start)
        {
            if (switchState is BeholderSwitchMoveState characterSwitchMoveState)
                characterSwitchMoveState.Start = start;

            return this;
        }
        public BeholderSwitchSwitchMoveStateBuilder SetEnd(Transform end)
        {
            if (switchState is BeholderSwitchMoveState characterSwitchMoveState)
                characterSwitchMoveState.End = end;

            return this;
        }

        public BeholderSwitchSwitchMoveStateBuilder SetCharacterController(CharacterController characterController)
        {
            if (switchState is BeholderSwitchMoveState characterSwitchMoveState)
                characterSwitchMoveState.CharacterController = characterController;

            return this;
        }
    }
}