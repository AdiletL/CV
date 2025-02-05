using Movement;
using Photon.Pun;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerRunState : CharacterRunState
    {
        private PhotonView photonView;
        private CharacterController characterController;
        private Rotation rotation;
        
        private Vector3 directionMovement;
        private float runReductionEndurance;
        private float rotationSpeed;
        
        public void SetCharacterController(CharacterController characterController) => this.characterController = characterController;
        public void SetRotationSpeed(float speed) => this.rotationSpeed = speed;
        public void SetPhotonView(PhotonView photonView) => this.photonView = photonView;
        public void SetRunReductionEndurance(float runReductionEndurance) => this.runReductionEndurance = runReductionEndurance;


        public override void Initialize()
        {
            base.Initialize();
            rotation = new Rotation(gameObject.transform, rotationSpeed);
        }
        
        public override void Subscribe()
        {
            base.Subscribe();
            StateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            StateMachine.OnExitCategory -= OnExitCategory;
        }

        public override void Update()
        {
            base.Update();
            
            CheckDirectionMovement();
            if (directionMovement.magnitude == 0)
            {
                StateMachine.ExitCategory(Category, null);
                return;
            }
            
            Rotate();
            ExecuteMovement();
        }
        
        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
                PlayAnimation();
        }

        private void CheckDirectionMovement()
        {
            directionMovement = Vector3.zero;
            
            if (Input.GetKey(KeyCode.A)) directionMovement.x = -1;
            if (Input.GetKey(KeyCode.D)) directionMovement.x = 1;
            if (Input.GetKey(KeyCode.W)) directionMovement.z = 1;
            if (Input.GetKey(KeyCode.S)) directionMovement.z = -1;
        }
        
        public override void ExecuteMovement()
        {
            base.ExecuteMovement();
            if (directionMovement.magnitude > 0)
            {
                characterController.Move(directionMovement * (MovementSpeed * Time.deltaTime));
                unitEndurance.RemoveEndurance(runReductionEndurance);
            }
        }

        private void Rotate()
        {
            if (directionMovement.magnitude > 0)
                rotation.RotateToDirection(directionMovement);
        }
    }

    public class PlayerRunStateBuilder : CharacterRunStateBuilder
    {
        public PlayerRunStateBuilder() : base(new PlayerRunState())
        {
        }

        public PlayerRunStateBuilder SetCharacterController(CharacterController characterController)
        {
            if(state is PlayerRunState playerRunStateOrig)
                playerRunStateOrig.SetCharacterController(characterController);
            return this;
        }
        
        public PlayerRunStateBuilder SetRotationSpeed(float speed)
        {
            if(state is PlayerRunState playerRunStateOrig)
                playerRunStateOrig.SetRotationSpeed(speed);
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
    }
}