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
        private DiContainer diContainer;
        private SO_SkillContainer so_SkillContainer;
        
        private HandleSkill handleSkill;
        private StateMachine stateMachine;
        private GameObject gameObject;
        private PlayerSwitchAttack playerSwitchAttack;
        private PlayerIdleState playerIdleState;
        private PlayerSwitchMove playerSwitchMove;
        private SO_PlayerControlDesktop so_PlayerControlDesktop;
        private SO_PlayerMove so_PlayerMove;
        private CharacterController characterController;
        private Gravity gravity;
        
        private RaycastHit[] hits = new RaycastHit[1];

        private GameObject previousHit;
        private Texture2D selectAttackTexture;
        private LayerMask enemyLayer;

        private bool isSelectedAttack;
        
        public PlayerControlDesktop(HandleSkill handleSkill, StateMachine stateMachine, GameObject gameObject, PlayerSwitchAttack playerSwitchAttack, 
            PlayerSwitchMove playerSwitchMove, SO_PlayerControlDesktop so_PlayerControlDesktop, SO_PlayerMove so_PlayerMove, 
            CharacterController characterController, LayerMask enemyLayer)
        {
            this.handleSkill = handleSkill;
            this.stateMachine = stateMachine;
            this.gameObject = gameObject;
            this.so_PlayerControlDesktop = so_PlayerControlDesktop;
            this.so_PlayerMove = so_PlayerMove;
            this.playerSwitchMove = playerSwitchMove;
            this.playerSwitchAttack = playerSwitchAttack;
            this.characterController = characterController;
            this.enemyLayer = enemyLayer;
        }

        [Inject]
        private void Construct(DiContainer diContainer, SO_SkillContainer so_SkillContainer)
        {
            this.diContainer = diContainer;
            this.so_SkillContainer = so_SkillContainer;
        }

        private async UniTask<Dash> CreateDash()
        {
            var so_Dash = await so_SkillContainer.GetSkillConfig<SO_SkillDash>();
            if(!so_Dash) return null;

            return (Dash)new DashBuilder()
                .SetCharacterController(characterController)
                .SetDuration(so_Dash.DashDuration)
                .SetSpeed(so_Dash.DashSpeed)
                .SetGameObject(gameObject)
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
            gravity = gameObject.GetComponent<PlayerGravity>();
            selectAttackTexture = so_PlayerControlDesktop.SelectAttackIcon.texture;
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
                if (!handleSkill.IsSkillNotNull(typeof(Dash)))
                {
                    var dash = await CreateDash();
                    diContainer.Inject(dash);
                    handleSkill.AddSkill(dash);
                }
                
                gravity.InActivateGravity();
                stateMachine.ExitOtherStates(typeof(PlayerIdleState), true);
                stateMachine.ActiveBlockChangeState();
                handleSkill.Execute(typeof(Dash), AfterDash);
            }
        }

        public override void HandleInput()
        {
            if (isSelectedAttack && Input.GetMouseButtonDown(0))
            {
                if (tryGetHitPosition(out GameObject hitObject, enemyLayer))
                {
                    playerSwitchAttack.SetTarget(hitObject);
                    playerSwitchAttack.ExitOtherStates();
                }
                ClearHotkey();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (tryGetHitPosition(out GameObject hitObject, Layers.CELL_LAYER))
                {
                    playerSwitchMove.SetTarget(hitObject);
                    playerSwitchMove.ExitOtherStates();
                }

                ClearHotkey();
            }
        }

        private void AfterDash()
        {
            gravity.ActivateGravity();
            stateMachine.InActiveBlockChangeState();
        }
    }
}