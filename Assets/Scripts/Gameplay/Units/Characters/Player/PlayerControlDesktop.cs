using System;
using System.Collections.Generic;
using Gameplay.Skill;
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
    public class PlayerControlDesktop : CharacterControlDesktop
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeyses;

        private event Action OnHandleInput;
        

        private IClickableObject selectObject;
        private IInputHandler playerSkillInputHandler;

        private RaycastHit[] hits = new RaycastHit[5];
        private Texture2D selectAttackTexture;
        
        private KeyCode jumpKey;
        private KeyCode attackKey;
        private int selectObjectMousButton, attackMouseButton;
        private int selectCellMouseButton;
        
        private bool isSelectedAttack;
        
        private readonly List<IInteractionHandler> interactionHandlers = new();

        public PhotonView PhotonView { get; set; }
        public PlayerController PlayerController { get; set; }
        public StateMachine StateMachine { get; set; }
        public GameObject GameObject { get; set; }
        public PlayerAnimation PlayerAnimation { get; set; }
        public ISwitchState PlayerSwitchAttack { get; set; }
        public ISwitchState PlayerSwitchMove { get; set; }
        public IEndurance Endurance { get; set; }
        public SO_PlayerControlDesktop SO_PlayerControlDesktop { get; set; }
        public SO_PlayerMove SO_PlayerMove { get; set; }
        public CharacterController CharacterController { get; set; }
        public LayerMask EnemyLayer { get; set; }
        public bool IsJumping { get; private set; }
        public bool IsUseSkill { get; private set; }
        
        ~PlayerControlDesktop()
        {
            UnInitializeMediator();
            UnRegisterInteraction();
        }

        
        private PlayerJumpState CreateJumpState()
        {
            return (PlayerJumpState)new PlayerJumpStateBuilder()
                .SetEndurance(Endurance)
                .SetJumpKey(so_GameHotkeyses.JumpKey)
                .SetReductionEndurance(SO_PlayerMove.JumpInfo.BaseReductionEndurance)
                .SetCharacterController(CharacterController)
                .SetMaxJumpCount(SO_PlayerMove.JumpInfo.MaxCount)
                .SetJumpClip(SO_PlayerMove.JumpInfo.Clip)
                .SetJumpHeight(SO_PlayerMove.JumpInfo.Height)
                .SetGameObject(GameObject)
                .SetCharacterAnimation(PlayerAnimation)
                .SetStateMachine(StateMachine)
                .Build();
        }

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
            
            playerSkillInputHandler = new PlayerSkillInputHandler(GameObject, StateMachine, this, CharacterController);
            diContainer.Inject(playerSkillInputHandler);
            playerSkillInputHandler.Initialize();
            
            InitializeMediator();
            
            selectAttackTexture = SO_PlayerControlDesktop.SelectAttackIcon.texture;
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
            var containerInteractionHandler = new ContainerInteractionHandler(GameObject, this);
            var lootInteractionHandler = new LootInteractionHandler(GameObject, this);
            RegisterInteraction(containerInteractionHandler);
            RegisterInteraction(lootInteractionHandler);
        }
        public void RegisterInteraction(IInteractionHandler handler)
        {
            diContainer.Inject(handler);
            handler.Initialize();
            interactionHandlers.Add(handler);
            OnHandleInput += handler.HandleInput;
            PlayerController.TriggerEnter += handler.CheckTriggerEnter;
            PlayerController.TriggerExit += handler.CheckTriggerExit;
        }
        
        public void UnRegisterInteraction()
        {
            foreach (var VARIABLE in interactionHandlers)
            {
                OnHandleInput -= VARIABLE.HandleInput;
                PlayerController.TriggerEnter -= VARIABLE.CheckTriggerEnter;
                PlayerController.TriggerExit -= VARIABLE.CheckTriggerExit;
            }
        }

        private void InitializeJumpState()
        {
            if (!StateMachine.IsStateNotNull(typeof(PlayerJumpState)))
            {
                var jumpState = CreateJumpState();
                jumpState.Initialize();
                StateMachine.AddStates(jumpState);
            }
        }

        private void InitializeMediator()
        {
            StateMachine.OnExitCategory += OnExitCategory;
            OnHandleInput += playerSkillInputHandler.HandleInput;
        }

        private void UnInitializeMediator()
        {
            StateMachine.OnExitCategory -= OnExitCategory;
            OnHandleInput -= playerSkillInputHandler.HandleInput;
        }

        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                IsJumping = false;
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
            if(!PhotonView.IsMine) return;
            if(!playerSkillInputHandler.IsCanInput()) return;
            
            base.HandleHotkey();
            
            if (Input.GetKeyDown(attackKey) && !IsJumping)
            {
                isSelectedAttack = true;
                Cursor.SetCursor(selectAttackTexture, Vector2.zero, CursorMode.Auto);
                ClearSelectObject();
            }
            else if (Input.GetKeyDown(jumpKey) && !IsJumping)
            {
                TriggerJump();
            }
        }

        public override void HandleInput()
        {
            if(!PhotonView.IsMine) return;
            if(!playerSkillInputHandler.IsCanInput()) return;
            
            base.HandleInput();

            if (Input.GetMouseButtonDown(selectCellMouseButton))
            {
                TriggerSelectCell();
            }
            else if (isSelectedAttack && Input.GetMouseButtonDown(attackMouseButton) && !IsJumping)
            {
                TriggerAttack();
            }
            else if (Input.GetMouseButtonDown(selectObjectMousButton) && !IsJumping)
            {
                TriggerSelectObject();
            }
            
            OnHandleInput?.Invoke();
        }
        
        private void TriggerSelectCell()
        {
            if (tryGetHitPosition<CellController>(out GameObject hitObject, Layers.CELL_LAYER))
            {
                PlayerSwitchMove.SetTarget(hitObject);
                PlayerSwitchMove.ExitOtherStates();
            }
            ClearHotkeys();
        }
        
        private void TriggerAttack()
        {
            if (tryGetHitPosition<IPlayerAttackable>(out GameObject gameObject, EnemyLayer))
            { 
                PlayerSwitchAttack.SetTarget(gameObject);
                PlayerSwitchAttack.ExitOtherStates();
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
            StateMachine.ExitCategory(StateCategory.attack, null);
            StateMachine.ExitCategory(StateCategory.action, null);
            StateMachine.ExitCategory(StateCategory.idle, typeof(PlayerJumpState));
            IsJumping = true;
        }

    }
    
    public class PlayerControlDesktopBuilder : CharacterControlDesktopBuilder
    {
        public PlayerControlDesktopBuilder() : base(new PlayerControlDesktop())
        {
        }

        public PlayerControlDesktopBuilder SetPhotonView(PhotonView view)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.PhotonView = view;
            return this;
        }
        public PlayerControlDesktopBuilder SetPlayerController(PlayerController playerController)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.PlayerController = playerController;
            return this;
        }
        public PlayerControlDesktopBuilder SetStateMachine(StateMachine stateMachine)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.StateMachine = stateMachine;
            return this;
        }

        public PlayerControlDesktopBuilder SetGameObject(GameObject gameObject)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.GameObject = gameObject;
            return this;
        }

        public PlayerControlDesktopBuilder SetPlayerSwitchAttack(ISwitchState playerSwitchAttack)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.PlayerSwitchAttack = playerSwitchAttack;
            return this;
        }

        public PlayerControlDesktopBuilder SetPlayerSwitchMove(ISwitchState playerSwitchMove)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.PlayerSwitchMove = playerSwitchMove;
            return this;
        }

        public PlayerControlDesktopBuilder SetPlayerControlDesktopConfig(SO_PlayerControlDesktop soPlayerControlDesktop)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SO_PlayerControlDesktop = soPlayerControlDesktop;
            return this;
        }

        public PlayerControlDesktopBuilder SetPlayerMoveConfig(SO_PlayerMove soPlayerMove)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.SO_PlayerMove = soPlayerMove;
            return this;
        }

        public PlayerControlDesktopBuilder SetCharacterController(CharacterController characterController)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.CharacterController = characterController;
            return this;
        }
        
        public PlayerControlDesktopBuilder SetEndurance(IEndurance playerEndurance)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.Endurance = playerEndurance;
            return this;
        }
        
        public PlayerControlDesktopBuilder SetPlayerAnimation(PlayerAnimation playerAnimation)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.PlayerAnimation = playerAnimation;
            return this;
        }

        public PlayerControlDesktopBuilder SetEnemyLayer(LayerMask enemyLayer)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.EnemyLayer = enemyLayer;
            return this;
        }
    }
}
