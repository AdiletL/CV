using System;
using System.Collections.Generic;
using Gameplay.Factory.Character.Player;
using Gameplay.Unit.Item.Container;
using Gameplay.Unit.Item;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using Unit;
using UnityEngine;
using Zenject;

[Flags]
public enum InputType
{
    Nothing = 0,
    Everything = ~0,
    Movement = 1 << 0,
    Jump = 1 << 1,
    SelectObject = 1 << 2,
    Attack = 1 << 3,
    SpecialAction = 1 << 4,
    Item = 1 << 5,
    Ability = 1 << 6,
}
namespace Gameplay.Unit.Character.Player
{
    public class PlayerControlDesktop : CharacterControlDesktop
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeyse;
        private event Action OnHandleInput;
        
        private PhotonView photonView;
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
            InitializeJumpState();
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
            if (typeof(CharacterMoveState).IsAssignableFrom(state.GetType()))
            {
                playerBlockInput.UnblockInput(movementBlockInputType);
            }
            else if (typeof(CharacterJumpState).IsAssignableFrom(state.GetType()))
            {
                playerBlockInput.UnblockInput(jumpBlockInputType);
            }
        }

        public override void ClearHotkeys()
        {
            base.ClearHotkeys();
            playerMouseInputHandler.ClearSelectedObject();
        }


        public override void HandleHotkey()
        {
            if(!photonView.IsMine || !isCanControl) return;
            base.HandleHotkey();
            
        }

        public override void HandleInput()
        {
            if(!photonView.IsMine || !isCanControl) return;
            base.HandleInput();
            
            if ((Input.GetKey(KeyCode.A) || 
                 Input.GetKey(KeyCode.D) || 
                 Input.GetKey(KeyCode.W) || 
                 Input.GetKey(KeyCode.S)) && 
                !playerBlockInput.IsInputBlocked(InputType.Movement))
            {
                playerBlockInput.BlockInput(movementBlockInputType);
                stateMachine.ExitOtherStates(typeof(CharacterMoveState));
            }
            else if (Input.GetKeyDown(jumpKey) &&
                     !playerBlockInput.IsInputBlocked(InputType.Jump))
            {
                TriggerJump();
            }
            
            OnHandleInput?.Invoke();
        }
        
        private void TriggerJump()
        {
            ClearHotkeys();
            stateMachine.ExitOtherStates(typeof(CharacterJumpState), true);
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
            if (characterControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetConfig(config);
            return this;
        }
        public PlayerControlDesktopBuilder SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl)
        {
            if (characterControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerKinematicControl(playerKinematicControl);
            return this;
        }
        public PlayerControlDesktopBuilder SetPlayerStateFactory(PlayerStateFactory playerStateFactory)
        {
            if (characterControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerStateFactory(playerStateFactory);
            return this;
        }
        public PlayerControlDesktopBuilder SetPhotonView(PhotonView view)
        {
            if (characterControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPhotonView(view);
            return this;
        }
        public PlayerControlDesktopBuilder SetPlayerController(PlayerController playerController)
        {
            if (characterControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerController(playerController);
            return this;
        }
        public PlayerControlDesktopBuilder SetStateMachine(StateMachine stateMachine)
        {
            if (characterControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetStateMachine(stateMachine);
            return this;
        }
        public PlayerControlDesktopBuilder SetPlayerAttackConfig(SO_PlayerAttack config)
        {
            if (characterControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerAttackConfig(config);
            return this;
        }
        
        public PlayerControlDesktopBuilder SetPlayerSpecialActionConfig(SO_PlayerSpecialAction config)
        {
            if (characterControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerSpecialActionConfig(config);
            return this;
        }
        
        public PlayerControlDesktopBuilder SetPlayerMoveConfig(SO_PlayerMove config)
        {
            if (characterControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetPlayerMoveConfig(config);
            return this;
        }
    }
}
