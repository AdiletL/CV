using System;
using System.Collections.Generic;
using Gameplay.Factory;
using Gameplay.Skill;
using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerSkillInputHandler : IInputHandler
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeys;

        private SkillFactory skillFactory;
        private SkillHandler skillHandler;
        private KeyCode dashKey;

        private readonly GameObject gameObject;
        private readonly StateMachine stateMachine;
        private readonly CharacterControlDesktop characterControlDesktop;
        private readonly IMoveControl moveControl;
        private readonly DashConfig dashConfig;
        private readonly Camera baseCamera;
        
        private Dictionary<InputType, int> blockedInputs = new();
        private Dictionary<SkillType, int> blockedSkills = new();

        public PlayerSkillInputHandler(GameObject gameObject, StateMachine stateMachine, 
            CharacterControlDesktop characterControlDesktop, IMoveControl moveControl,
            DashConfig dashConfig, Camera baseCamera)
        {
            this.gameObject = gameObject;
            this.stateMachine = stateMachine;
            this.characterControlDesktop = characterControlDesktop;
            this.moveControl = moveControl;
            this.dashConfig = dashConfig;
            this.baseCamera = baseCamera;
        }

        private SkillFactory CreateSkillFactory()
        {
            return new SkillFactoryBuilder(new SkillFactory())
                .SetBaseCamera(baseCamera)
                .SetGameObject(gameObject)
                .SetMoveControl(moveControl)
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

        public void Initialize()
        {
            skillHandler = gameObject.GetComponent<SkillHandler>();
            dashKey = so_GameHotkeys.DashKey;
            skillFactory = CreateSkillFactory();
            diContainer.Inject(skillFactory);
            InitializeDash();
        }
                
        private void InitializeDash()
        {
            if (!skillHandler.IsSkillNotNull(SkillType.dash))
            {
                var dash = skillFactory.CreateSkill(dashConfig);
                diContainer.Inject(dash);
                skillHandler.AddSkill(dash);
            }
        }
        
        public void HandleInput()
        {
            if (Input.GetKeyDown(dashKey) &&
                !isSkillBlocked(SkillType.dash))
            {
                TriggerDash();
            }
        }
        
        private void TriggerDash()
        {
            InitializeDash();

            this.stateMachine.ExitOtherStates(typeof(PlayerIdleState), true);
            this.stateMachine.ActiveBlockChangeState();
            skillHandler.Execute(SkillType.dash, 0, AfterDash);
            characterControlDesktop.ClearHotkeys();
            BlockInput(skillHandler.GetSkill(SkillType.dash, 0).BlockedInputType);
            BlockSkill(skillHandler.GetSkill(SkillType.dash, 0).BlockedSkillType);
        }
                
        private void AfterDash()
        {
            this.stateMachine.InActiveBlockChangeState();
            UnblockInput(skillHandler.GetSkill(SkillType.dash, 0).BlockedInputType);
            UnblockSkill(skillHandler.GetSkill(SkillType.dash, 0).BlockedSkillType);
        }
    }
}