using System;
using System.Collections.Generic;
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

        private readonly GameObject gameObject;
        private readonly StateMachine stateMachine;
        private readonly CharacterController characterController;
        private readonly CharacterControlDesktop characterControlDesktop;
        
        private Dictionary<InputType, int> blockedInputs = new();
        private Dictionary<SkillType, int> blockedSkills = new();

        public PlayerSkillInputHandler(GameObject gameObject, StateMachine stateMachine, 
            CharacterControlDesktop characterControlDesktop, CharacterController characterController)
        {
            this.gameObject = gameObject;
            this.stateMachine = stateMachine;
            this.characterControlDesktop = characterControlDesktop;
            this.characterController = characterController;
        }

        private async UniTask<Dash> CreateDash()
        {
            var so_Dash = await so_SkillContainer.GetSkillConfig<SO_SkillDash>();
            if(!so_Dash) return null;

            return (Dash)new DashBuilder()
                .SetCharacterController(characterController)
                .SetDuration(so_Dash.DashDuration)
                .SetSpeed(so_Dash.DashSpeed)
                .SetBlockedInputType(so_Dash.BlockedInputType)
                .SetBlockedSkillType(so_Dash.BlockedSkillType)
                .SetGameObject(gameObject)
                .Build();
        }

        /// <summary>
        /// Проверяет, заблокирован ли ввод.
        /// </summary>
        public bool IsInputBlocked(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.nothing || (input & flag) == 0) continue;

                if (blockedInputs.ContainsKey(flag) && blockedInputs[flag] > 0)
                    return true;
            }
            return false;
        }
        private bool isSkillBlocked(SkillType input)
        {
            foreach (SkillType flag in Enum.GetValues(typeof(SkillType)))
            {
                if (flag == SkillType.nothing || (input & flag) == 0) continue;

                if (blockedSkills.ContainsKey(flag) && blockedSkills[flag] > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Блокирует ввод (увеличивает счётчик блокировок).
        /// </summary>
        public void BlockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.nothing || (input & flag) == 0) continue;

                if (!blockedInputs.ContainsKey(flag))
                    blockedInputs[flag] = 0;

                blockedInputs[flag]++;
            }
        }
       
        /// <summary>
        /// Разблокирует ввод (уменьшает счётчик блокировок).
        /// </summary>
        public void UnblockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.nothing || (input & flag) == 0) continue;

                if (blockedInputs.ContainsKey(flag))
                {
                    blockedInputs[flag]--;

                    if (blockedInputs[flag] <= 0) 
                        blockedInputs.Remove(flag);
                }
            }
        }
        
        private void BlockSkill(SkillType input)
        {
            foreach (SkillType flag in Enum.GetValues(typeof(SkillType)))
            {
                if (flag == SkillType.nothing || (input & flag) == 0) continue;

                if (!blockedSkills.ContainsKey(flag))
                    blockedSkills[flag] = 0;

                blockedSkills[flag]++;
            }
        }
        
        private void UnblockSkill(SkillType input)
        {
            foreach (SkillType flag in Enum.GetValues(typeof(SkillType)))
            {
                if (flag == SkillType.nothing || (input & flag) == 0) continue;

                if (blockedSkills.ContainsKey(flag))
                {
                    blockedSkills[flag]--;

                    if (blockedSkills[flag] <= 0) 
                        blockedSkills.Remove(flag);
                }
            }
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
            if (Input.GetKeyDown(dashKey) &&
                !isSkillBlocked(SkillType.dash))
            {
                await TriggerDash();
            }
        }
        
        private async UniTask TriggerDash()
        {
            await InitializeDash();

            gravity.InActivateGravity();
            this.stateMachine.ExitOtherStates(typeof(PlayerIdleState), true);
            this.stateMachine.ActiveBlockChangeState();
            skillHandler.Execute(typeof(Dash), AfterDash);
            characterControlDesktop.ClearHotkeys();
            BlockInput(skillHandler.GetSkill(typeof(Dash)).BlockedInputType);
            BlockSkill(skillHandler.GetSkill(typeof(Dash)).BlockedSkillType);
        }
                
        private void AfterDash()
        {
            gravity.ActivateGravity();
            this.stateMachine.InActiveBlockChangeState();
            UnblockInput(skillHandler.GetSkill(typeof(Dash)).BlockedInputType);
            UnblockSkill(skillHandler.GetSkill(typeof(Dash)).BlockedSkillType);
        }
    }
}