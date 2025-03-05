using System;
using System.Collections.Generic;
using Gameplay.Factory.Character.Player;
using Gameplay.Units.Item;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using Unit.Item.Container;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerControlDesktop : CharacterControlDesktop
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeyse;
        private event Action OnHandleInput;
        public static event Action<InputType> OnBlockInput; 
        
        private PhotonView photonView;
        private CharacterSwitchAttackState characterSwitchAttack;
        private CharacterSwitchMoveState characterSwitchMove;
        private StateMachine stateMachine;

        private SO_PlayerControlDesktop so_PlayerControlerDesktop;
        private PlayerController playerController;
        private PlayerStateFactory playerStateFactory;
        private PlayerKinematicControl playerKinematicControl;
        private PlayerMouseInputHandler playerMouseInputHandler;
        private PlayerBlockInput playerBlockInput;
        
        private SO_PlayerAttack so_PlayerAttack;
        private SO_PlayerSpecialAction so_PlayerSpecialAction;
        private SO_PlayerMove so_PlayerMove;

        private InputType movementBlockInputType;
        private InputType jumpBlockInputType;
        private KeyCode jumpKey;
        
        private bool isJumping;
        private bool isMoving;
        
        private readonly List<IInteractionHandler> interactionHandlers = new();

        
        ~PlayerControlDesktop()
        {
            UnSubscribeEvent();
        }
        
        public void SetConfig(SO_PlayerControlDesktop config) => so_PlayerControlerDesktop = config;
        public void SetPlayerKinematicControl(PlayerKinematicControl playerControl) => this.playerKinematicControl = playerControl;
        public void SetPlayerStateFactory(PlayerStateFactory playerStateFactory) => this.playerStateFactory = playerStateFactory;
        public void SetPhotonView(PhotonView photonView) => this.photonView = photonView;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterSwitchAttack(CharacterSwitchAttackState characterSwitchAttackState) =>
            this.characterSwitchAttack = characterSwitchAttackState;
        public void SetCharacterSwitchMove(CharacterSwitchMoveState characterSwitchMoveState) => this.characterSwitchMove = characterSwitchMoveState;
        public void SetPlayerController(PlayerController playerController) => this.playerController = playerController;
        public void SetPlayerAttackConfig(SO_PlayerAttack config) => this.so_PlayerAttack = config;
        public void SetPlayerSpecialActionConfig(SO_PlayerSpecialAction config) => this.so_PlayerSpecialAction = config;
        public void SetPlayerMoveConfig(SO_PlayerMove config) => this.so_PlayerMove = config;
        
        

        public override void Initialize()
        {
            base.Initialize();

            movementBlockInputType = so_PlayerMove.BlockInputType;
            jumpBlockInputType = so_PlayerMove.JumpConfig.BlockInputType;
            playerBlockInput = playerController.PlayerBlockInput;
            
            InitializeHotkeys();
            InitializeInteractionHandler();
            InitializeMouseInputHandler();
            SubscribeEvent();
        }

        private void InitializeHotkeys()
        {
            jumpKey = so_GameHotkeyse.JumpKey;
        }

        private void InitializeInteractionHandler()
        {
            var containerInteractionHandler = new ContainerInteractionHandler(gameObject, this);
            var lootInteractionHandler = new ItemInteractionHandler(gameObject, this);
            RegisterInteraction(containerInteractionHandler);
            RegisterInteraction(lootInteractionHandler);
        }
        private void RegisterInteraction(IInteractionHandler handler)
        {
            diContainer.Inject(handler);
            handler.Initialize();
            interactionHandlers.Add(handler);
        }

        private void InitializeMouseInputHandler()
        {
            playerMouseInputHandler = new PlayerMouseInputHandlerBuilder()
                .SetStateMachine(stateMachine)
                .SetPlayerStateFactory(playerStateFactory)
                .SetAttackBlockInput(so_PlayerAttack.BlockInputType)
                .SetSpecialBlockInput(so_PlayerSpecialAction.BlockInputType)
                .SetPlayerBlockInput(playerBlockInput)
                .SetCharacterControlDesktop(this)
                .SetCharacterSwitchAttackState(characterSwitchAttack)
                .Build();
            diContainer.Inject(playerMouseInputHandler);
            playerMouseInputHandler.Initialize();
        }
        
        private void InitializeJumpState()
        {
            if (!stateMachine.IsStateNotNull(typeof(PlayerJumpState)))
            {
                var jumpState = playerStateFactory.CreateState(typeof(PlayerJumpState));
                diContainer.Inject(jumpState);
                jumpState.Initialize();
                stateMachine.AddStates(jumpState);
            }
        }

        private void SubscribeEvent()
        {
            stateMachine.OnExitCategory += OnExitCategory;
            OnHandleInput += playerMouseInputHandler.HandleInput;
            
            foreach (var VARIABLE in interactionHandlers)
            {
                OnHandleInput += VARIABLE.HandleInput;
                playerController.TriggerEnter += VARIABLE.CheckTriggerEnter;
                playerController.TriggerExit += VARIABLE.CheckTriggerExit;
            }
        }

        private void UnSubscribeEvent()
        {
            stateMachine.OnExitCategory -= OnExitCategory;
            OnHandleInput -= playerMouseInputHandler.HandleInput;
            
            foreach (var VARIABLE in interactionHandlers)
            {
                OnHandleInput -= VARIABLE.HandleInput;
                playerController.TriggerEnter -= VARIABLE.CheckTriggerEnter;
                playerController.TriggerExit -= VARIABLE.CheckTriggerExit;
            }
        }

        private void OnExitCategory(IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                isJumping = false;
                playerBlockInput.UnblockInput(jumpBlockInputType);
            }

            if (state.GetType().IsAssignableFrom(typeof(PlayerRunState)))
            {
                isMoving = false;
                playerBlockInput.UnblockInput(movementBlockInputType);
            }
        }

        public override void ClearHotkeys()
        {
            base.ClearHotkeys();
            playerMouseInputHandler.ClearSelectedObject();
        }


        public override void HandleHotkey()
        {
            if(!photonView.IsMine) return;
            base.HandleHotkey();
            
        }

        public override void HandleInput()
        {
            if(!photonView.IsMine) return;
            base.HandleInput();
            
            if (!isMoving &&
                (Input.GetKey(KeyCode.A) || 
                Input.GetKey(KeyCode.D) || 
                Input.GetKey(KeyCode.W) || 
                Input.GetKey(KeyCode.S)) && 
                !playerBlockInput.IsInputBlocked(InputType.Movement))
            {
                isMoving = true;
                playerBlockInput.BlockInput(movementBlockInputType);
                characterSwitchMove.ExitOtherStates();
            }
            else if (!isJumping && Input.GetKeyDown(jumpKey) &&
                     !playerBlockInput.IsInputBlocked(InputType.Jump))
            {
                TriggerJump();
            }
            
            OnHandleInput?.Invoke();
        }
        
        private void TriggerJump()
        {
            ClearHotkeys();
            InitializeJumpState();
            stateMachine.ExitOtherStates(typeof(PlayerJumpState), true);
            isJumping = true;
            playerBlockInput.BlockInput(jumpBlockInputType);
        }
    }
    
    public class PlayerControlDesktopBuilder : CharacterControlDesktopBuilder
    {
        public PlayerControlDesktopBuilder() : base(new PlayerControlDesktop())
        {
        }

        public PlayerControlDesktopBuilder SetConfig(SO_PlayerControlDesktop config)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetConfig(config);
            return this;
        }
        public PlayerControlDesktopBuilder SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerKinematicControl(playerKinematicControl);
            return this;
        }
        public PlayerControlDesktopBuilder SetPlayerStateFactory(PlayerStateFactory playerStateFactory)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerStateFactory(playerStateFactory);
            return this;
        }
        public PlayerControlDesktopBuilder SetPhotonView(PhotonView view)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPhotonView(view);
            return this;
        }
        public PlayerControlDesktopBuilder SetPlayerController(PlayerController playerController)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerController(playerController);
            return this;
        }
        public PlayerControlDesktopBuilder SetStateMachine(StateMachine stateMachine)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetStateMachine(stateMachine);
            return this;
        }

        public PlayerControlDesktopBuilder SetCharacterSwitchAttack(CharacterSwitchAttackState playerSwitchAttack)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetCharacterSwitchAttack(playerSwitchAttack);
            return this;
        }

        public PlayerControlDesktopBuilder SetCharacterSwitchMove(CharacterSwitchMoveState playerSwitchMove)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetCharacterSwitchMove(playerSwitchMove);
            return this;
        }
        
        public PlayerControlDesktopBuilder SetPlayerAttackConfig(SO_PlayerAttack config)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerAttackConfig(config);
            return this;
        }
        
        public PlayerControlDesktopBuilder SetPlayerSpecialActionConfig(SO_PlayerSpecialAction config)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerSpecialActionConfig(config);
            return this;
        }
        
        public PlayerControlDesktopBuilder SetPlayerMoveConfig(SO_PlayerMove config)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerMoveConfig(config);
            return this;
        }
    }
}
