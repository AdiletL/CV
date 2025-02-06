using System;
using System.Collections.Generic;
using Gameplay.Factory.Character.Player;
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
        
        private PlayerKinematicControl playerKinematicControl;
        private PhotonView photonView;
        private PlayerController playerController;
        private PlayerStateFactory playerStateFactory;
        private CharacterSwitchAttackState characterSwitchAttack;
        private CharacterSwitchMoveState characterSwitchMove;
        private SO_PlayerControlDesktop so_PlayerControlDesktop;
        private StateMachine stateMachine;

        private IClickableObject selectedObject;
        private UnitRenderer selectedRenderer;
        private UnitRenderer highlightedRenderer;
        private PlayerSkillInputHandler playerSkillInputHandler;

        private RaycastHit[] hits = new RaycastHit[5];
        private Texture2D selectAttackTexture;
        private LayerMask enemyLayer;

        private KeyCode jumpKey;
        private int selectObjectMousButton, attackMouseButton;
        private int hitRayOnObjectCount, hitsCount;

        private const float cooldownHighlighObject = .2f;
        private float countCooldownHighlighObject;
        
        private bool isJumping;
        private bool isMoving;
        private bool isAttacking;
        
        private readonly List<IInteractionHandler> interactionHandlers = new();

        ~PlayerControlDesktop()
        {
            UnInitializeMediator();
            UnRegisterInteraction();
        }
        
        
        public void SetPlayerKinematicControl(PlayerKinematicControl playerControl) => this.playerKinematicControl = playerControl;
        public void SetPlayerStateFactory(PlayerStateFactory playerStateFactory) => this.playerStateFactory = playerStateFactory;
        public void SetPhotonView(PhotonView photonView) => this.photonView = photonView;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
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

            hitsCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, layerMask);

            for (int i = 0; i < hitsCount; i++)
            {
                if (hits[i].transform.TryGetComponent(out T component))
                {
                    float targetDistance = hits[i].distance;
                    if (targetDistance < closestDistance)
                    {
                        closestDistance = targetDistance;
                        closestHit = hits[i].transform.gameObject;
                    }
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
            playerSkillInputHandler = new PlayerSkillInputHandler(gameObject, stateMachine, 
                this, playerKinematicControl);
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
            if (stateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)) || 
                stateMachine.IsStateNotNull(typeof(PlayerDefaultAttackState)))
            {
                isAttacking = false;
            }
            
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                isJumping = false;
            }

            if (state.GetType().IsAssignableFrom(typeof(PlayerRunState)))
            {
                isMoving = false;
            }
        }

        public override void ClearHotkeys()
        {
            base.ClearHotkeys();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            ClearSelectObject();
        }

        private void ClearSelectObject()
        {
            selectedObject?.UnSelectObject();
            selectedObject?.HideInformation();
            selectedRenderer?.UnSelectedObject();
            selectedObject = null;
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
                !playerSkillInputHandler.IsInputBlocked(InputType.movement))
            {
                characterSwitchMove.ExitOtherStates();
                isMoving = true;
            }
            else if (Input.GetMouseButtonDown(attackMouseButton) && 
                     !isJumping && !isAttacking && 
                     !playerSkillInputHandler.IsInputBlocked(InputType.attack))
            {
                TriggerAttack();
            }
            else if (Input.GetMouseButtonDown(selectObjectMousButton) && 
                     !isJumping && !playerSkillInputHandler.IsInputBlocked(InputType.selectObject))
            {
                TriggerSelectObject();
            }
            else if (Input.GetKeyDown(jumpKey) && !isJumping &&
                !playerSkillInputHandler.IsInputBlocked(InputType.jump))
            {
                TriggerJump();
            }
            
            OnHandleInput?.Invoke();
        }

        public override void HandleHighlight()
        {
            countCooldownHighlighObject += Time.deltaTime;
            
            if(countCooldownHighlighObject < cooldownHighlighObject) return;
            countCooldownHighlighObject = 0;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hitRayOnObjectCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, Layers.CREEP_LAYER | Layers.PLAYER_LAYER);

            UnitRenderer newHighlightedRenderer = null;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < hitRayOnObjectCount; i++)
            {
                if (hits[i].transform.TryGetComponent(out UnitRenderer unitRenderer))
                {
                    float targetDistance = hits[i].distance;
                    if (targetDistance < closestDistance)
                    {
                        closestDistance = targetDistance;
                        newHighlightedRenderer = unitRenderer;
                    }
                }
            }

            if (newHighlightedRenderer == highlightedRenderer) return; // Если объект тот же, выходим

            highlightedRenderer?.UnHighlightedObject(); // Снимаем подсветку с предыдущего
            highlightedRenderer = newHighlightedRenderer;
            highlightedRenderer?.HighlightedObject();   // Подсвечиваем новый
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
                //characterSwitchAttack.SetTarget(gameObject);
            }

            isAttacking = true;
            characterSwitchAttack.ExitOtherStates();
            ClearHotkeys();
        }
        
        private void TriggerSelectObject()
        {
            if (tryGetHitPosition<IClickableObject>(out GameObject hitObject, Layers.EVERYTHING_LAYER))
            {
                var clickableObject = hitObject.GetComponent<IClickableObject>();
                clickableObject.UpdateInformation();
                if (selectedObject == null || selectedObject != clickableObject)
                {
                    selectedObject?.UnSelectObject();
                    selectedRenderer?.UnSelectedObject();
                    
                    selectedObject = clickableObject;
                    selectedObject.ShowInformation();
                    selectedObject.SelectObject();
                    selectedRenderer = hitObject.GetComponent<UnitRenderer>();
                    selectedRenderer.SelectedObject();
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
            stateMachine.ExitOtherStates(typeof(PlayerJumpState), true);
            //stateMachine.ExitCategory(StateCategory.attack, null);
            //stateMachine.ExitCategory(StateCategory.action, null);
            //stateMachine.ExitCategory(StateCategory.idle, typeof(PlayerJumpState));
            isJumping = true;
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

        public PlayerControlDesktopBuilder SetPlayerControlDesktopConfig(SO_PlayerControlDesktop soPlayerControlDesktop)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SetConfig(soPlayerControlDesktop);
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
