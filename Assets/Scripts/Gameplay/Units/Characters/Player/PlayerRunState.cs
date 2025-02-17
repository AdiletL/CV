using Movement;
using Photon.Pun;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerRunState : CharacterRunState
    {
        private PhotonView photonView;
        private PlayerKinematicControl playerKinematicControl;
        
        private Vector3 directionMovement;
        private float runReductionEndurance;
        private float currentRotationSpeed;
        private float baseRotationSpeed;
        
        public void SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl) => this.playerKinematicControl = playerKinematicControl;
        public void SetBaseRotationSpeed(float speed) => this.baseRotationSpeed = speed;
        public void SetPhotonView(PhotonView photonView) => this.photonView = photonView;
        public void SetRunReductionEndurance(float runReductionEndurance) => this.runReductionEndurance = runReductionEndurance;


        public override void Initialize()
        {
            base.Initialize();
            currentRotationSpeed = baseRotationSpeed;
        }

        public override void Enter()
        {
            base.Enter();
            ResetValue();
            if (stateMachine.IsActivateType(typeof(PlayerSpecialActionState)))
                ChangedStateOnPlayerSpecialActionState();
        }

        public override void Subscribe()
        {
            base.Subscribe();
            stateMachine.OnExitCategory += OnExitCategory;
            stateMachine.OnChangedState += OnChangedState;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            stateMachine.OnExitCategory -= OnExitCategory;
            stateMachine.OnChangedState -= OnChangedState;
        }

        public override void Update()
        {
            base.Update();
            
            CheckDirectionMovement();
            Rotate();
            ExecuteMovement();

            if (directionMovement.magnitude == 0)
                stateMachine.ExitCategory(Category, typeof(PlayerIdleState));
        }

        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
                PlayAnimation();
            
            if (state.GetType().IsAssignableFrom(typeof(PlayerSpecialActionState)))
                ResetValue();
        }

        private void OnChangedState(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerSpecialActionState)))
                ChangedStateOnPlayerSpecialActionState();
        }

        private void ChangedStateOnPlayerSpecialActionState()
        {
            CurrentMovementSpeed /= 2;
            currentRotationSpeed = 0;
            playerKinematicControl.SetRotationSpeed(currentRotationSpeed);
        }

        private void ResetValue()
        {
            CurrentMovementSpeed = BaseMovementSpeed;
            currentRotationSpeed = baseRotationSpeed;
            playerKinematicControl.SetRotationSpeed(currentRotationSpeed);
        }

        private void CheckDirectionMovement()
        {
            directionMovement = Vector3.zero;
            
            if (Input.GetKey(KeyCode.A)) directionMovement.x = -1;
            if (Input.GetKey(KeyCode.D)) directionMovement.x = 1;
            if (Input.GetKey(KeyCode.W)) directionMovement.z = 1;
            if (Input.GetKey(KeyCode.S)) directionMovement.z = -1;
            
            if(directionMovement.magnitude > 0)
                directionMovement.Normalize();
        }
        
        public override void ExecuteMovement()
        {
            base.ExecuteMovement();
            playerKinematicControl.SetVelocity(directionMovement * (CurrentMovementSpeed));
            unitEndurance.RemoveEndurance(runReductionEndurance);
        }

        private void Rotate()
        {
            playerKinematicControl.SetDirectionRotate(directionMovement);
        }
    }

    public class PlayerRunStateBuilder : CharacterRunStateBuilder
    {
        public PlayerRunStateBuilder() : base(new PlayerRunState())
        {
        }
        
        public PlayerRunStateBuilder SetRotationSpeed(float speed)
        {
            if(state is PlayerRunState playerRunStateOrig)
                playerRunStateOrig.SetBaseRotationSpeed(speed);
            return this;
        }
        
        public PlayerRunStateBuilder SetPhotonView(PhotonView photonView)
        {
            if(state is PlayerRunState playerRunStateOrig)
                playerRunStateOrig.SetPhotonView(photonView);
            return this;
        }
        
        public PlayerRunStateBuilder SetReductionEndurance(float reductionEndurance)
        {
            if(state is PlayerRunState playerRunStateOrig)
                playerRunStateOrig.SetRunReductionEndurance(reductionEndurance);
            return this;
        }
        
        public PlayerRunStateBuilder SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl)
        {
            if(state is PlayerRunState playerRunStateOrig)
                playerRunStateOrig.SetPlayerKinematicControl(playerKinematicControl);
            return this;
        }
    }
}