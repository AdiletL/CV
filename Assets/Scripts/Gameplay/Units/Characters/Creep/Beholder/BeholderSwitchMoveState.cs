using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderSwitchMoveState : CreepSwitchMoveState
    {
        private SO_BeholderMove so_BeholderMove;
        private BeholderAnimation beholderAnimation;
        private GameObject currentTarget;
        
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
        }
        
        protected override void DestermineState()
        {
            base.DestermineState();
            if (currentTarget)
            {
                if(!this.StateMachine.IsStateNotNull(typeof(BeholderRunState)))
                {
                    var runState = CreateRunState();
                    runState.Initialize();
                    this.StateMachine.AddStates(runState);
                }
                
                this.StateMachine.GetState<BeholderRunState>().SetTarget(currentTarget);
                this.StateMachine.SetStates(typeof(BeholderRunState));
                currentTarget = null;
            }
            else 
            {
                if (!this.StateMachine.IsStateNotNull(typeof(BeholderPatrolState)))
                {
                    var patrolState = CreatePatrolState();
                    patrolState.Initialize();
                    this.StateMachine.AddStates(patrolState);
                }
                
                this.StateMachine.SetStates(typeof(BeholderPatrolState));
            }
        }
        
        public void SetTarget(GameObject target)
        {
            currentTarget = target;
            if (this.StateMachine.IsStateNotNull(typeof(BeholderRunState)))
            {
                this.StateMachine.GetState<BeholderRunState>().SetTarget(currentTarget);
            }
        }
    }
    
    public class BeholderSwitchMoveStateBuilder : CreepSwitchMoveStateBuilder
    {
        public BeholderSwitchMoveStateBuilder() : base(new BeholderSwitchMoveState())
        {
        }
        
        public BeholderSwitchMoveStateBuilder SetStart(Transform start)
        {
            if (state is BeholderSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.Start = start;
            }

            return this;
        }
        public BeholderSwitchMoveStateBuilder SetEnd(Transform end)
        {
            if (state is BeholderSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.End = end;
            }

            return this;
        }

        public BeholderSwitchMoveStateBuilder SetCharacterController(CharacterController characterController)
        {
            if (state is BeholderSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.CharacterController = characterController;
            }

            return this;
        }
    }
}