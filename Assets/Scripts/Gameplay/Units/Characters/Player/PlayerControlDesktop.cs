using System;
using System.Collections.Generic;
using Gameplay.Factory;
using Gameplay.Units.Item.Loot;
using Machine;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using Unit.Cell;
using Unit.Item.Container;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    [Flags]
    public enum InputType
    {
        nothing,
        movement = 1 << 0,
        jump = 1 << 1,
        selectObject = 1 << 2,
        attack = 1 << 3,
    }
    public class PlayerControlDesktop : CharacterControlDesktop
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeyses;
        private event Action OnHandleInput;
        
        private PhotonView photonView;
        private PlayerController playerController;
        private PlayerStateFactory playerStateFactory;
        private CharacterSwitchAttackState characterSwitchAttack;
        private CharacterSwitchMoveState characterSwitchMove;
        private SO_PlayerControlDesktop so_PlayerControlDesktop;
        private CharacterController characterController;
        private StateMachine stateMachine;
        private LayerMask enemyLayer;

        private IClickableObject selectObject;
        private PlayerSkillInputHandler playerSkillInputHandler;

        private RaycastHit[] hits = new RaycastHit[5];
        private Texture2D selectAttackTexture;
        
        private KeyCode jumpKey;
        private KeyCode attackKey;
        private int selectObjectMousButton, attackMouseButton;
        private int selectCellMouseButton;
        
        private bool isSelectedAttack;
        private bool isJumping;
        
        private readonly List<IInteractionHandler> interactionHandlers = new();

        ~PlayerControlDesktop()
        {
            UnInitializeMediator();
            UnRegisterInteraction();
        }
        
        public void SetPlayerStateFactory(PlayerStateFactory playerStateFactory) => this.playerStateFactory = playerStateFactory;
        public void SetPhotonView(PhotonView photonView) => this.photonView = photonView;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterController(CharacterController characterController) => this.characterController = characterController;
        public void SetCharacterSwitchAttack(CharacterSwitchAttackState characterSwitchAttackState) =>
            this.characterSwitchAttack = characterSwitchAttackState;
        public void SetCharacterSwitchMove(CharacterSwitchMoveState characterSwitchMoveState) => this.characterSwitchMove = characterSwitchMoveState;
        public void SetConfig(SO_PlayerControlDesktop config) => this.so_PlayerControlDesktop = config;
        public void SetPlayerController(PlayerController playerController) => this.playerController = playerController;
        public void SetEnemyLayer(LayerMask layerMask) => enemyLayer = layerMask;
        
        
        private bool tryGetHitPosition<T>(out GameObject hitObject, LayerMask layerMask)
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            float closestDistance = Mathf.Infinity; // Ищем ближайший объект
            GameObject closestHit = null;

            var hitsCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, layerMask);

            for (int i = 0; i < hitsCount; i++)
            {
                float targetDistance = Vector3.Distance(ray.origin, hits[i].point);
                
                if (targetDistance < closestDistance && hits[i].transform.TryGetComponent(out T component))
                {
                    closestDistance = targetDistance;
                    closestHit = hits[i].transform.gameObject;
                }
            }

            if (closestHit != null)
            {
                hitObject = closestHit;
                return true;
            }

            hitObject = default;
            return false;
        }
        

        public override void Initialize()
        {
            base.Initialize();

            InitializeHotkeys();
            InitializeInteractionHandler();
            InitializeSkillInputHandler();
            InitializeMediator();
            
            selectAttackTexture = so_PlayerControlDesktop.SelectAttackIcon.texture;
        }

        private void InitializeHotkeys()
        {
            selectCellMouseButton = so_GameHotkeyses.SelectCellMouseButton;
            attackKey = so_GameHotkeyses.AttackKey;
            attackMouseButton = so_GameHotkeyses.AttackMouseButton;
            selectObjectMousButton = so_GameHotkeyses.SelectObjectMouseButton;
            jumpKey = so_GameHotkeyses.JumpKey;
        }

        private void InitializeInteractionHandler()
        {
            var containerInteractionHandler = new ContainerInteractionHandler(gameObject, this);
            var lootInteractionHandler = new LootInteractionHandler(gameObject, this);
            RegisterInteraction(containerInteractionHandler);
            RegisterInteraction(lootInteractionHandler);
        }
        public void RegisterInteraction(IInteractionHandler handler)
        {
            diContainer.Inject(handler);
            handler.Initialize();
            interactionHandlers.Add(handler);
            OnHandleInput += handler.HandleInput;
            playerController.TriggerEnter += handler.CheckTriggerEnter;
            playerController.TriggerExit += handler.CheckTriggerExit;
        }
        
        public void UnRegisterInteraction()
        {
            foreach (var VARIABLE in interactionHandlers)
            {
                OnHandleInput -= VARIABLE.HandleInput;
                playerController.TriggerEnter -= VARIABLE.CheckTriggerEnter;
                playerController.TriggerExit -= VARIABLE.CheckTriggerExit;
            }
        }

        private void InitializeSkillInputHandler()
        {
            playerSkillInputHandler = new PlayerSkillInputHandler(gameObject, stateMachine, this, characterController);
            diContainer.Inject(playerSkillInputHandler);
            playerSkillInputHandler.Initialize();
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
            OnHandleInput += playerSkillInputHandler.HandleInput;
        }

        private void UnInitializeMediator()
        {
            stateMachine.OnExitCategory -= OnExitCategory;
            OnHandleInput -= playerSkillInputHandler.HandleInput;
        }

        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                isJumping = false;
            }
        }

        public override void ClearHotkeys()
        {
            base.ClearHotkeys();
            isSelectedAttack = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            ClearSelectObject();
        }

        private void ClearSelectObject()
        {
            selectObject?.HideInformation();
            selectObject = null;
        }

        public override void HandleHotkey()
        {
            if(!photonView.IsMine) return;
            base.HandleHotkey();
            
            if (Input.GetKeyDown(attackKey) && !isJumping)
            {
                isSelectedAttack = true;
                Cursor.SetCursor(selectAttackTexture, Vector2.zero, CursorMode.Auto);
                ClearSelectObject();
            }
            else if (Input.GetKeyDown(jumpKey) && !isJumping &&
                     !playerSkillInputHandler.IsInputBlocked(InputType.jump))
            {
                TriggerJump();
            }
        }

        public override void HandleInput()
        {
            if(!photonView.IsMine) return;
            base.HandleInput();

            if (Input.GetMouseButtonDown(selectCellMouseButton) && !playerSkillInputHandler.IsInputBlocked(InputType.movement))
            {
                TriggerSelectCell();
            }
            else if (isSelectedAttack && Input.GetMouseButtonDown(attackMouseButton) && 
                     !isJumping && !playerSkillInputHandler.IsInputBlocked(InputType.attack))
            {
                TriggerAttack();
            }
            else if (Input.GetMouseButtonDown(selectObjectMousButton) && 
                     !isJumping && !playerSkillInputHandler.IsInputBlocked(InputType.selectObject))
            {
                TriggerSelectObject();
            }
            
            OnHandleInput?.Invoke();
        }
        
        private void TriggerSelectCell()
        {
            if (tryGetHitPosition<CellController>(out GameObject hitObject, Layers.CELL_LAYER))
            { 
                characterSwitchMove.SetTarget(hitObject);
                characterSwitchMove.ExitOtherStates();
            }
            ClearHotkeys();
        }
        
        private void TriggerAttack()
        {
            if (tryGetHitPosition<IPlayerAttackable>(out GameObject gameObject, enemyLayer))
            { 
                characterSwitchAttack.SetTarget(gameObject);
                characterSwitchAttack.ExitOtherStates();
            }
            ClearHotkeys();
        }
        
        private void TriggerSelectObject()
        {
            if (tryGetHitPosition<IClickableObject>(out GameObject hitObject, Layers.EVERYTHING_LAYER))
            {
                var unit = hitObject.GetComponent<IClickableObject>();
                unit.UpdateInformation();
                if (selectObject == null || selectObject != unit)
                {
                    selectObject = unit;
                    selectObject.ShowInformation();
                }
                else
                {
                    ClearHotkeys();
                }
            }
        }
        
        private void TriggerJump()
        {
            ClearHotkeys();
            ClearSelectObject();
            InitializeJumpState();
            stateMachine.ExitCategory(StateCategory.attack, null);
            stateMachine.ExitCategory(StateCategory.action, null);
            stateMachine.ExitCategory(StateCategory.idle, typeof(PlayerJumpState));
            isJumping = true;
        }

    }
    
    public class PlayerControlDesktopBuilder : CharacterControlDesktopBuilder
    {
        public PlayerControlDesktopBuilder() : base(new PlayerControlDesktop())
        {
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

        public PlayerControlDesktopBuilder SetPlayerControlDesktopConfig(SO_PlayerControlDesktop soPlayerControlDesktop)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetConfig(soPlayerControlDesktop);
            return this;
        }

        public PlayerControlDesktopBuilder SetCharacterController(CharacterController characterController)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetCharacterController(characterController);
            return this;
        }
        
        public PlayerControlDesktopBuilder SetEnemyLayer(LayerMask enemyLayer)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetEnemyLayer(enemyLayer);
            return this;
        }
    }

}
