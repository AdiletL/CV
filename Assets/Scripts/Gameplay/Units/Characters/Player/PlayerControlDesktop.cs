using Cysharp.Threading.Tasks;
using Gameplay.Skill;
using ScriptableObjects.Gameplay.Skill;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerControlDesktop : CharacterControlDesktop
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_SkillContainer so_SkillContainer;
        
        private Gravity gravity;
        private RaycastHit[] hits = new RaycastHit[1];
        private Texture2D selectAttackTexture;
        private bool isSelectedAttack;
        
        public HandleSkill HandleSkill { get; set; }
        public StateMachine StateMachine { get; set; }
        public GameObject GameObject { get; set; }
        public PlayerSwitchAttack PlayerSwitchAttack { get; set; }
        public PlayerSwitchMove PlayerSwitchMove { get; set; }
        public SO_PlayerControlDesktop SO_PlayerControlDesktop { get; set; }
        public SO_PlayerMove SO_PlayerMove { get; set; }
        public CharacterController CharacterController { get; set; }
        public LayerMask EnemyLayer { get; set; }
        

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

        private bool tryGetHitPosition(out GameObject hitObject, LayerMask layerMask)
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            var hitsCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, layerMask);
            for (int i = 0; i < hitsCount; i++)
            {
                hitObject = hits[i].transform.gameObject;
                return true;
            }

            hitObject = null;
            return false;
        }
        

        public override void Initialize()
        {
            base.Initialize();
            gravity = GameObject.GetComponent<PlayerGravity>();
            selectAttackTexture = SO_PlayerControlDesktop.SelectAttackIcon.texture;
        }

        protected override void ClearHotkey()
        {
            base.ClearHotkey();
            isSelectedAttack = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public override async void HandleHotkey()
        {
            base.HandleHotkey();
            if (Input.GetKeyDown(KeyCode.A))
            {
                isSelectedAttack = true;
                Cursor.SetCursor(selectAttackTexture, Vector2.zero, CursorMode.Auto);
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (!HandleSkill.IsSkillNotNull(typeof(Dash)))
                {
                    var dash = await CreateDash();
                    diContainer.Inject(dash);
                    HandleSkill.AddSkill(dash);
                }
                
                gravity.InActivateGravity();
                StateMachine.ExitOtherStates(typeof(PlayerIdleState), true);
                StateMachine.ActiveBlockChangeState();
                HandleSkill.Execute(typeof(Dash), AfterDash);
            }
        }

        public override void HandleInput()
        {
            if (isSelectedAttack && Input.GetMouseButtonDown(0))
            {
                if (tryGetHitPosition(out GameObject hitObject, EnemyLayer))
                {
                    PlayerSwitchAttack.SetTarget(hitObject);
                    PlayerSwitchAttack.ExitOtherStates();
                }
                ClearHotkey();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (tryGetHitPosition(out GameObject hitObject, Layers.CELL_LAYER))
                {
                    PlayerSwitchMove.SetTarget(hitObject);
                    PlayerSwitchMove.ExitOtherStates();
                }

                ClearHotkey();
            }
        }

        private void AfterDash()
        {
            gravity.ActivateGravity();
            StateMachine.InActiveBlockChangeState();
        }
    }

    namespace Unit.Character.Player
{
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

        public PlayerControlDesktopBuilder SetEnemyLayer(LayerMask enemyLayer)
        {
            if (unitControlDesktop is PlayerControlDesktop playerControlDesktop)
                playerControlDesktop.EnemyLayer = enemyLayer;
            return this;
        }
    }
}
}