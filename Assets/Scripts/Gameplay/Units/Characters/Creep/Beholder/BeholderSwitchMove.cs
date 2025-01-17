using Machine;
using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderSwitchMove : CreepSwitchMove
    {
        private SO_BeholderMove so_BeholderMove;
        private BeholderAnimation beholderAnimation;
        private BeholderRunState beholderRunState;
        private BeholderPatrolState beholderPatrolState;
        private BeholderSwitchAttack beholderSwitchAttack;
        
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
                .SetCenter(Center)
                .SetEnemyAnimation(beholderAnimation)
                .SetWalkClip(so_BeholderMove.WalkClip)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetCharacterSwitchAttack(beholderSwitchAttack)
                .SetStart(Start)
                .SetEnd(End)
                .SetGameObject(GameObject)
                .SetMovementSpeed(so_BeholderMove.WalkSpeed)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        private BeholderRunState CreateRunState()
        {
            return (BeholderRunState)new BeholderRunStateBuilder()
                .SetCenter(Center)
                .SetCharacterController(CharacterController)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetCharacterAnimation(beholderAnimation)
                .SetCharacterSwitchAttack(beholderSwitchAttack)
                .SetRunClips(so_BeholderMove.RunClip)
                .SetGameObject(GameObject)
                .SetMovementSpeed(so_BeholderMove.RunSpeed)
                .SetStateMachine(this.StateMachine)
                .Build();
        }
        

        public override void Initialize()
        {
            base.Initialize();
            so_BeholderMove = (SO_BeholderMove)SO_CharacterMove;
            beholderAnimation = (BeholderAnimation)CharacterAnimation;
            beholderSwitchAttack = (BeholderSwitchAttack)CharacterSwitchAttack;
        }

        public override void SetState()
        {
            base.SetState();
            if (currentTarget)
            {
                if(!this.StateMachine.IsStateNotNull(typeof(BeholderRunState)))
                {
                    beholderRunState = CreateRunState();
                    beholderRunState.Initialize();
                    this.StateMachine.AddStates(beholderRunState);
                }
                
                beholderRunState.SetTarget(currentTarget);
                if(!this.StateMachine.IsActivateType(beholderRunState.GetType()))
                    this.StateMachine.SetStates(beholderRunState.GetType());
            }
            else 
            {
                if (!this.StateMachine.IsStateNotNull(typeof(BeholderPatrolState)))
                {
                    beholderPatrolState = CreatePatrolState();
                    beholderPatrolState.Initialize();
                    this.StateMachine.AddStates(beholderPatrolState);
                }
                
                if(!this.StateMachine.IsActivateType(beholderPatrolState.GetType()))
                    this.StateMachine.SetStates(beholderPatrolState.GetType());
            }
            currentTarget = null;
        }

        public override void ExitCategory(StateCategory category)
        {
            base.ExitCategory(category);
            if (currentTarget)
            {
                if(!this.StateMachine.IsStateNotNull(typeof(BeholderRunState)))
                {
                    beholderRunState = CreateRunState();
                    beholderRunState.Initialize();
                    this.StateMachine.AddStates(beholderRunState);
                }
                
                beholderRunState.SetTarget(currentTarget);
                if(!this.StateMachine.IsActivateType(beholderRunState.GetType()))
                    this.StateMachine.ExitCategory(category, beholderRunState.GetType());
            }
            else 
            {
                if (!this.StateMachine.IsStateNotNull(typeof(BeholderPatrolState)))
                {
                    beholderPatrolState = CreatePatrolState();
                    beholderPatrolState.Initialize();
                    this.StateMachine.AddStates(beholderPatrolState);
                }
                
                if(!this.StateMachine.IsActivateType(beholderPatrolState.GetType()))
                    this.StateMachine.ExitCategory(category, beholderPatrolState.GetType());
            }
            currentTarget = null;
        }

        public override void ExitOtherStates()
        {
            base.ExitOtherStates();
            if (currentTarget)
            {
                if(!this.StateMachine.IsStateNotNull(typeof(BeholderRunState)))
                {
                    beholderRunState = CreateRunState();
                    beholderRunState.Initialize();
                    this.StateMachine.AddStates(beholderRunState);
                }
                
                beholderRunState.SetTarget(currentTarget);
                if(!this.StateMachine.IsActivateType(beholderRunState.GetType()))
                    this.StateMachine.ExitOtherStates(beholderRunState.GetType());
            }
            else 
            {
                if (!this.StateMachine.IsStateNotNull(typeof(BeholderPatrolState)))
                {
                    beholderPatrolState = CreatePatrolState();
                    beholderPatrolState.Initialize();
                    this.StateMachine.AddStates(beholderPatrolState);
                }
                
                if(!this.StateMachine.IsActivateType(beholderPatrolState.GetType()))
                    this.StateMachine.ExitOtherStates(beholderPatrolState.GetType());
            }
            currentTarget = null;
        }
    }
    
    public class BeholderSwitchSwitchMoveBuilder : CreepSwitchSwitchMoveBuilder
    {
        public BeholderSwitchSwitchMoveBuilder() : base(new BeholderSwitchMove())
        {
        }
        
        public BeholderSwitchSwitchMoveBuilder SetStart(Transform start)
        {
            if (state is BeholderSwitchMove characterSwitchMoveState)
            {
                characterSwitchMoveState.Start = start;
            }

            return this;
        }
        public BeholderSwitchSwitchMoveBuilder SetEnd(Transform end)
        {
            if (state is BeholderSwitchMove characterSwitchMoveState)
            {
                characterSwitchMoveState.End = end;
            }

            return this;
        }

        public BeholderSwitchSwitchMoveBuilder SetCharacterController(CharacterController characterController)
        {
            if (state is BeholderSwitchMove characterSwitchMoveState)
            {
                characterSwitchMoveState.CharacterController = characterController;
            }

            return this;
        }
    }
}