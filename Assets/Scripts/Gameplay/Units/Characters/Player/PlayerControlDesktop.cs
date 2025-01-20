using Cysharp.Threading.Tasks;
using Gameplay.Skill;
using Machine;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Skill;
using ScriptableObjects.Unit.Character.Player;
using Unit.Cell;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerControlDesktop : CharacterControlDesktop
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_SkillContainer so_SkillContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeyses;

        private IClickableObject selectObject;
        
        private Gravity gravity;
        private RaycastHit[] hits = new RaycastHit[5];
        private Texture2D selectAttackTexture;
        
        private KeyCode dashKey, jumpKey, attackKey;
        private int selectObjectMousButton, attackMouseButton, selectCellMouseButton;
        
        private bool isSelectedAttack;
        private bool isJumping;
        private bool isDashing;
        
        public HandleSkill HandleSkill { get; set; }
        public StateMachine StateMachine { get; set; }
        public GameObject GameObject { get; set; }
        public PlayerAnimation PlayerAnimation { get; set; }
        public PlayerSwitchAttack PlayerSwitchAttack { get; set; }
        public PlayerSwitchMove PlayerSwitchMove { get; set; }
        public PlayerEndurance PlayerEndurance { get; set; }
        public SO_PlayerControlDesktop SO_PlayerControlDesktop { get; set; }
        public SO_PlayerMove SO_PlayerMove { get; set; }
        public CharacterController CharacterController { get; set; }
        public LayerMask EnemyLayer { get; set; }
        
        ~PlayerControlDesktop()
        {
            DeInitializeMediator();
        }

        private async UniTask<Dash> CreateDash()
        {
            var so_Dash = await so_SkillContainer.GetSkillConfig<SO_SkillDash>();
            if(!so_Dash) return null;

            return (Dash)new DashBuilder()
                .SetCharacterController(CharacterController)
                .SetDuration(so_Dash.DashDuration)
                .SetSpeed(so_Dash.DashSpeed)
                .SetGameObject(GameObject)
                .Build();
        }
        
        private PlayerJumpState CreateJumpState()
        {
            return (PlayerJumpState)new PlayerJumpStateBuilder()
                .SetPlayerEndurance(PlayerEndurance)
                .SetJumpKey(so_GameHotkeyses.JumpKey)
                .SetDecreaseEndurance(SO_PlayerMove.JumpInfo.DecreaseEndurance)
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

            InitializeMediator();
            
            gravity = GameObject.GetComponent<PlayerGravity>();
            selectAttackTexture = SO_PlayerControlDesktop.SelectAttackIcon.texture;

            InitializeHotkeys();
        }

        private void InitializeHotkeys()
        {
            selectCellMouseButton = so_GameHotkeyses.SelectCellMouseButton;
            attackKey = so_GameHotkeyses.AttackKey;
            attackMouseButton = so_GameHotkeyses.AttackMouseButton;
            selectObjectMousButton = so_GameHotkeyses.SelectObjectMouseButton;
            jumpKey = so_GameHotkeyses.JumpKey;
            dashKey = so_GameHotkeyses.DashKey;
        }
        
        private async UniTask InitializeDash()
        {
            if (!HandleSkill.IsSkillNotNull(typeof(Dash)))
            {
                var dash = await CreateDash();
                diContainer.Inject(dash);
                HandleSkill.AddSkill(dash);
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
        }

        private void DeInitializeMediator()
        {
            StateMachine.OnExitCategory -= OnExitCategory;
        }

        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                isJumping = false;
            }
        }

        protected override void ClearHotkeys()
        {
            base.ClearHotkeys();
            isSelectedAttack = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void ClearSelectObject()
        {
            selectObject?.HideInformation();
            selectObject = null;
        }

        public override async void HandleHotkey()
        {
            if(isDashing) return;
            
            base.HandleHotkey();
            
            if (Input.GetKeyDown(attackKey) && !isJumping)
            {
                isSelectedAttack = true;
                Cursor.SetCursor(selectAttackTexture, Vector2.zero, CursorMode.Auto);
                ClearSelectObject();
            }
            else if (Input.GetKeyDown(dashKey))
            {
                await TriggerDash();
            }
            else if (Input.GetKeyDown(jumpKey) && !isJumping)
            {
                TriggerJump();
            }
        }

        public override void HandleInput()
        {
            if(isDashing) return;
            
            base.HandleInput();

            if (Input.GetMouseButtonDown(selectCellMouseButton))
            {
                TriggerSelectCell();
            }
            else if (isSelectedAttack && Input.GetMouseButtonDown(attackMouseButton) && !isJumping)
            {
                TriggerAttack();
            }
            else if (Input.GetMouseButtonDown(selectObjectMousButton))
            {
                TriggerSelectObject();
            }
        }

        private async UniTask TriggerDash()
        {
            await InitializeDash();
                
            gravity.InActivateGravity();
            StateMachine.ExitOtherStates(typeof(PlayerIdleState), true);
            StateMachine.ActiveBlockChangeState();
            HandleSkill.Execute(typeof(Dash), AfterDash);
            ClearHotkeys();
            ClearSelectObject();
            isDashing = true;
        }
        
        private void TriggerSelectCell()
        {
            if (tryGetHitPosition<CellController>(out GameObject hitObject, Layers.CELL_LAYER))
            {
                PlayerSwitchMove.SetTarget(hitObject);
                PlayerSwitchMove.ExitOtherStates();
            }
            ClearHotkeys();
            ClearSelectObject();
        }
        
        private void TriggerAttack()
        {
            if (tryGetHitPosition<IPlayerAttackable>(out GameObject gameObject, EnemyLayer))
            {
                PlayerSwitchAttack.SetTarget(gameObject);
                PlayerSwitchAttack.ExitOtherStates();
            }
            ClearHotkeys();
            ClearSelectObject();
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
            }
            else
            {
                ClearSelectObject();
            }
            ClearHotkeys();
        }
        
        private void TriggerJump()
        {
            ClearHotkeys();
            ClearSelectObject();
            InitializeJumpState();
            StateMachine.ExitCategory(StateCategory.attack, null);
            StateMachine.ExitCategory(StateCategory.action, null);
            StateMachine.ExitCategory(StateCategory.idle, typeof(PlayerJumpState));
            isJumping = true;
        }

        private void AfterDash()
        {
            isDashing = false;
            gravity.ActivateGravity();
            StateMachine.InActiveBlockChangeState();
        }
    }
    
    public class PlayerControlDesktopBuilder : CharacterControlDesktopBuilder
    {
        public PlayerControlDesktopBuilder() : base(new PlayerControlDesktop())
        {
        }

        public PlayerControlDesktopBuilder SetHandleSkill(HandleSkill handleSkill)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.HandleSkill = handleSkill;
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

        public PlayerControlDesktopBuilder SetPlayerSwitchAttack(PlayerSwitchAttack playerSwitchAttack)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.PlayerSwitchAttack = playerSwitchAttack;
            return this;
        }

        public PlayerControlDesktopBuilder SetPlayerSwitchMove(PlayerSwitchMove playerSwitchMove)
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
        
        public PlayerControlDesktopBuilder SetPlayerEndurance(PlayerEndurance playerEndurance)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.PlayerEndurance = playerEndurance;
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
