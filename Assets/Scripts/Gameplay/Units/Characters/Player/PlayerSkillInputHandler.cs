using Cysharp.Threading.Tasks;
using Gameplay.Skill;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Skill;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerSkillInputHandler : IInputHandler
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_SkillContainer so_SkillContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeys;

        private SkillHandler skillHandler;
        private Gravity gravity;
        private KeyCode dashKey;

        private bool isDisableInput;

        private readonly GameObject gameObject;
        private readonly StateMachine stateMachine;
        private readonly CharacterController characterController;
        private readonly CharacterControlDesktop characterControlDesktop;


        public PlayerSkillInputHandler(GameObject gameObject, StateMachine stateMachine, 
            CharacterControlDesktop characterControlDesktop, CharacterController characterController)
        {
            this.gameObject = gameObject;
            this.stateMachine = stateMachine;
            this.characterControlDesktop = characterControlDesktop;
            this.characterController = characterController;
        }

        public bool IsCanInput()
        {
            return !isDisableInput;
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
        
        
        public async void Initialize()
        {
            gravity = gameObject.GetComponent<Gravity>();
            skillHandler = gameObject.GetComponent<SkillHandler>();
            dashKey = so_GameHotkeys.DashKey;
            await InitializeDash();
        }
        
                
        private async UniTask InitializeDash()
        {
            if (!skillHandler.IsSkillNotNull(typeof(Dash)))
            {
                var dash = await CreateDash();
                diContainer.Inject(dash);
                skillHandler.AddSkill(dash);
            }
        }
        
        public async void HandleInput()
        {
            if (!Input.GetKeyDown(dashKey)) return;
            
            await TriggerDash();
        }
        
        private async UniTask TriggerDash()
        {
            await InitializeDash();

            isDisableInput = true;
            gravity.InActivateGravity();
            this.stateMachine.ExitOtherStates(typeof(PlayerIdleState), true);
            this.stateMachine.ActiveBlockChangeState();
            skillHandler.Execute(typeof(Dash), AfterDash);
            characterControlDesktop.ClearHotkeys();
        }
                
        private void AfterDash()
        {
            gravity.ActivateGravity();
            this.stateMachine.InActiveBlockChangeState();
            isDisableInput = false;
        }
    }
}