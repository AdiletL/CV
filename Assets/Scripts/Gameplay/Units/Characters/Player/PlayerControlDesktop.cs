using System;
using System.Collections.Generic;
using Gameplay.Factory.Character.Player;
using Gameplay.Ability;
using Gameplay.Units.Item.Loot;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using Unit.Item.Container;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    [Flags]
    public enum InputType
    {
        Nothing,
        Movement = 1 << 0,
        Jump = 1 << 1,
        SelectObject = 1 << 2,
        Attack = 1 << 3,
        SpecialAction = 1 << 4,
    }
    
    public class PlayerControlDesktop : CharacterControlDesktop
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeyse;
        private event Action OnHandleInput;
        
        private PhotonView photonView;
        private CharacterSwitchAttackState characterSwitchAttack;
        private CharacterSwitchMoveState characterSwitchMove;
        private StateMachine stateMachine;

        private PlayerController playerController;
        private PlayerStateFactory playerStateFactory;
        private PlayerKinematicControl playerKinematicControl;
        private PlayerAbilityInventory playerAbilityInventory;
        private PlayerMouseInputHandler playerMouseInputHandler;
        private DashConfig dashConfig;

        private KeyCode jumpKey;

        private bool isJumping;
        private bool isMoving;
        
        private Dictionary<InputType, int> blockedInputs = new();
        private readonly List<IInteractionHandler> interactionHandlers = new();

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
        public void SetDashConfig(DashConfig dashConfig) => this.dashConfig = dashConfig;
        
        
        public override bool IsInputBlocked(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0) continue;

                if (blockedInputs.ContainsKey(flag) && blockedInputs[flag] > 0)
                    return true;
            }
            return false;
        }

        public void BlockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0) continue;

                if (!blockedInputs.ContainsKey(flag))
                    blockedInputs[flag] = 0;

                blockedInputs[flag]++;
            }
        }
        
        public void UnblockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0) continue;

                if (blockedInputs.ContainsKey(flag))
                {
                    blockedInputs[flag]--;

                    if (blockedInputs[flag] <= 0) 
                        blockedInputs.Remove(flag);
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            playerAbilityInventory = gameObject.GetComponent<PlayerAbilityInventory>();
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
            interactionHandlers.Add(handler);;
        }

        private void InitializeMouseInputHandler()
        {
            playerMouseInputHandler = new PlayerMouseInputHandler(stateMachine, this,
                playerAbilityInventory, characterSwitchAttack, gameObject.GetComponent<PlayerItemInventory>(), 
                playerStateFactory);
            diContainer.Inject(playerMouseInputHandler);
            playerMouseInputHandler.Initialize();
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
            OnHandleInput += playerMouseInputHandler.HandleInput;
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
            OnHandleInput -= playerMouseInputHandler.HandleInput;
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
                UnblockInput(InputType.Attack);
            }

            if (state.GetType().IsAssignableFrom(typeof(PlayerRunState)))
            {
                isMoving = false;
                UnblockInput(InputType.Attack);
            }
        }

        public override void ClearHotkeys()
        {
            base.ClearHotkeys();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
                !playerAbilityInventory.IsInputBlocked(InputType.Movement))
            {
                isMoving = true;
               BlockInput(InputType.Attack);
               characterSwitchMove.ExitOtherStates();
            }
            else if (!isJumping && Input.GetKeyDown(jumpKey) &&
                     !playerMouseInputHandler.IsInputBlocked(InputType.Jump) &&
                     !playerAbilityInventory.IsInputBlocked(InputType.Jump))
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
            BlockInput(InputType.Attack);
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
        
    }
}
