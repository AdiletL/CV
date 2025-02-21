using System;
using System.Collections.Generic;
using Gameplay.Factory.Character.Player;
using Gameplay.Ability;
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
        public static event Func<InputType, bool> OnIsInputBlocked;
        
        private PhotonView photonView;
        private CharacterSwitchAttackState characterSwitchAttack;
        private CharacterSwitchMoveState characterSwitchMove;
        private StateMachine stateMachine;

        private PlayerController playerController;
        private PlayerStateFactory playerStateFactory;
        private PlayerKinematicControl playerKinematicControl;
        
        private SO_PlayerAttack so_PlayerAttack;
        private SO_PlayerSpecialAction so_PlayerSpecialAction;
        private SO_PlayerMove so_PlayerMove;

        private InputType movementBlockInputType;
        private InputType jumpBlockInputType;
        private KeyCode jumpKey;
        
        private bool isJumping;
        private bool isMoving;
        
        private Dictionary<InputType, int> blockedInputs = new();
        private readonly List<IInteractionHandler> interactionHandlers = new();

        public PlayerMouseInputHandler PlayerMouseInputHandler { get; private set; }
        
        
        ~PlayerControlDesktop()
        {
            UnInitializeMediator();
        }
        
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
        
        
        private bool isCanUseControl(InputType input)
        {
            if (OnIsInputBlocked == null) return false;
            
            foreach (Func<InputType, bool> VARIABLE in OnIsInputBlocked.GetInvocationList())
            {
                if (VARIABLE.Invoke(input)) return false;
            }

            if (IsInputBlocked(InputType.Item)) return false;

            return true;
        }
        
        public override bool IsInputBlocked(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0 || 
                    flag == InputType.Everything) continue;

                if (blockedInputs.ContainsKey(flag) && blockedInputs[flag] > 0)
                    return true;
            }
            return false;
        }

        public void BlockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0 || 
                    flag == InputType.Everything) continue;

                blockedInputs.TryAdd(flag, 0);
                blockedInputs[flag]++;
            }
            OnBlockInput?.Invoke(input);
        }
        
        public void UnblockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0 || 
                    flag == InputType.Everything) continue;

                if (blockedInputs.ContainsKey(flag))
                {
                    blockedInputs[flag]--;

                    if (blockedInputs[flag] <= 0) 
                        blockedInputs.Remove(flag);
                }
            }
        }

        private bool OnInputBlockedFunc(InputType input)
        {
            return IsInputBlocked(input);
        }

        public override void Initialize()
        {
            base.Initialize();

            movementBlockInputType = so_PlayerMove.BlockInputType;
            jumpBlockInputType = so_PlayerMove.JumpInfo.BlockInputType;
            
            InitializeHotkeys();
            InitializeInteractionHandler();
            InitializeMouseInputHandler();
            InitializeMediator();
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
        public void RegisterInteraction(IInteractionHandler handler)
        {
            diContainer.Inject(handler);
            handler.Initialize();
            interactionHandlers.Add(handler);
        }

        private void InitializeMouseInputHandler()
        {
            PlayerMouseInputHandler = new PlayerMouseInputHandler(stateMachine, this, characterSwitchAttack, 
                playerStateFactory, so_PlayerAttack.BlockInputType, so_PlayerSpecialAction.BlockInputType);
            diContainer.Inject(PlayerMouseInputHandler);
            PlayerMouseInputHandler.Initialize();
        }
        
        private void InitializeJumpState()
        {
            if (!stateMachine.IsStateNotNull(typeof(PlayerJumpState)))
            {
                var jumpState = playerStateFactory.CreateState(typeof(PlayerJumpState));
                jumpState.Initialize();
                stateMachine.AddStates(jumpState);
            }
        }

        private void InitializeMediator()
        {
            stateMachine.OnExitCategory += OnExitCategory;
            OnHandleInput += PlayerMouseInputHandler.HandleInput;
            PlayerItemInventory.OnIsInputBlocked += OnInputBlockedFunc;
            PlayerAbilityInventory.OnIsInputBlocked += OnInputBlockedFunc;
            PlayerMouseInputHandler.OnIsInputBlocked += OnInputBlockedFunc;
            
            foreach (var VARIABLE in interactionHandlers)
            {
                OnHandleInput += VARIABLE.HandleInput;
                playerController.TriggerEnter += VARIABLE.CheckTriggerEnter;
                playerController.TriggerExit += VARIABLE.CheckTriggerExit;
            }
        }

        private void UnInitializeMediator()
        {
            stateMachine.OnExitCategory -= OnExitCategory;
            OnHandleInput -= PlayerMouseInputHandler.HandleInput;
            PlayerItemInventory.OnIsInputBlocked -= OnInputBlockedFunc;
            PlayerAbilityInventory.OnIsInputBlocked -= OnInputBlockedFunc;
            PlayerMouseInputHandler.OnIsInputBlocked -= OnInputBlockedFunc;
            
            foreach (var VARIABLE in interactionHandlers)
            {
                OnHandleInput -= VARIABLE.HandleInput;
                playerController.TriggerEnter -= VARIABLE.CheckTriggerEnter;
                playerController.TriggerExit -= VARIABLE.CheckTriggerExit;
            }
        }

        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                isJumping = false;
                UnblockInput(jumpBlockInputType);
            }

            if (state.GetType().IsAssignableFrom(typeof(PlayerRunState)))
            {
                isMoving = false;
                UnblockInput(movementBlockInputType);
            }
        }

        public override void ClearHotkeys()
        {
            base.ClearHotkeys();
            PlayerMouseInputHandler.ClearSelectedObject();
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
                isCanUseControl(InputType.Movement))
            {
                isMoving = true;
                BlockInput(movementBlockInputType);
                characterSwitchMove.ExitOtherStates();
            }
            else if (!isJumping && Input.GetKeyDown(jumpKey) &&
                     isCanUseControl(InputType.Jump))
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
            BlockInput(jumpBlockInputType);
        }
    }
    
    public class PlayerControlDesktopBuilder : CharacterControlDesktopBuilder
    {
        public PlayerControlDesktopBuilder() : base(new PlayerControlDesktop())
        {
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
