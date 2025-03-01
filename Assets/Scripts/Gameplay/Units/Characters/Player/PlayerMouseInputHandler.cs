﻿using System;
using System.Collections.Generic;
using Gameplay.Factory.Character.Player;
using Machine;
using ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerMouseInputHandler : IInputHandler
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeys;

        public static event Action<InputType> OnBlockInput;
        public static event Func<InputType, bool> OnIsInputBlocked;
        
        private PlayerStateFactory playerStateFactory;
        private CharacterControlDesktop characterControlDesktop;
        private CharacterSwitchAttackState characterSwitchAttack;
        private StateMachine stateMachine;
        
        private IClickableObject selectedObject;
        private UnitRenderer selectedRenderer;
        private UnitRenderer highlightedRenderer;
        private RaycastHit[] hits = new RaycastHit[5];

        private InputType attackBlockInputType;
        private InputType specialBlockInputType;
        
        private int selectObjectMousButton, attackMouseButton, specialActionMouseButton;
        private int hitRayOnObjectCount, hitsCount;
        
        private const float cooldownHighlighObject = .2f;
        private float countCooldownHighlighObject;
        
        private bool isAttacking;
        
        private Dictionary<InputType, int> blockedInputs = new();
        
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void CharacterControlDesktop(CharacterControlDesktop characterControlDesktop) => this.characterControlDesktop = characterControlDesktop;
        public void SetCharacterSwitchAttack(CharacterSwitchAttackState characterSwitchAttackState) =>
            this.characterSwitchAttack = characterSwitchAttackState;

        public PlayerMouseInputHandler(StateMachine stateMachine, CharacterControlDesktop characterControlDesktop,
            CharacterSwitchAttackState characterSwitchAttackState,
            PlayerStateFactory playerStateFactory, InputType attackBlockInputType, InputType specialBlockInputType)
        {
            this.stateMachine = stateMachine;
            this.characterControlDesktop = characterControlDesktop;
            this.characterSwitchAttack = characterSwitchAttackState;
            this.playerStateFactory = playerStateFactory;
            this.attackBlockInputType = attackBlockInputType;
            this.specialBlockInputType = specialBlockInputType;
        }

        ~PlayerMouseInputHandler()
        {
            UnInitializeMediator();
        }

        private bool isCanUseControl(InputType input)
        {
            if (OnIsInputBlocked == null) return false;
            
            foreach (Func<InputType, bool> VARIABLE in OnIsInputBlocked.GetInvocationList())
            {
                if (VARIABLE.Invoke(input)) return false;
            }

            if (IsInputBlocked(InputType.Item)) return false;

            return true;
        }
               
        // Оптимизированный метод для проверки, был ли клик по UI
        private bool IsPointerOverUIObject()
        {
            // Используем заранее созданные объекты
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            // Список результатов Raycast
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            // Получаем все результаты Raycast
            EventSystem.current.RaycastAll(pointerData, raycastResults);
            // Проверяем, есть ли хотя бы один объект в UI
            return raycastResults.Count > 0;
        }
        public bool IsInputBlocked(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0 || 
                    flag == InputType.Everything) continue;

                if (blockedInputs.ContainsKey(flag) && blockedInputs[flag] > 0)
                    return true;
            }
            return false;
        }
        
        public void BlockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0 || 
                    flag == InputType.Everything) continue;

                blockedInputs.TryAdd(flag, 0);
                blockedInputs[flag]++;
            }
            OnBlockInput?.Invoke(input);
        }
        
        public void UnblockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0 || 
                    flag == InputType.Everything) continue;

                if (blockedInputs.ContainsKey(flag))
                {
                    blockedInputs[flag]--;

                    if (blockedInputs[flag] <= 0) 
                        blockedInputs.Remove(flag);
                }
            }
        }

        
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
        
        private bool OnInputBlockedFunc(InputType input)
        {
            return IsInputBlocked(input);
        }
        
        public void Initialize()
        {
            attackMouseButton = so_GameHotkeys.AttackMouseButton;
            selectObjectMousButton = so_GameHotkeys.SelectObjectMouseButton;
            specialActionMouseButton = so_GameHotkeys.SpecialActionMouseButton;
            
            InitializeMediator();
        }

        private void InitializePlayerSpecialActionState()
        {
            if(stateMachine.IsStateNotNull(typeof(PlayerSpecialActionState))) return;
            var state = (PlayerSpecialActionState)playerStateFactory.CreateState(typeof(PlayerSpecialActionState));
            diContainer.Inject(state);
            state.Initialize();
            stateMachine.AddStates(state);
        }

        private void InitializeMediator()
        {
            stateMachine.OnExitCategory += OnExitCategory;
            PlayerItemInventory.OnIsInputBlocked += OnInputBlockedFunc;
            PlayerAbilityInventory.OnIsInputBlocked += OnInputBlockedFunc;
            PlayerControlDesktop.OnIsInputBlocked += OnInputBlockedFunc;
        }

        private void UnInitializeMediator()
        {
            stateMachine.OnExitCategory -= OnExitCategory;
            PlayerItemInventory.OnIsInputBlocked -= OnInputBlockedFunc;
            PlayerAbilityInventory.OnIsInputBlocked -= OnInputBlockedFunc;
            PlayerControlDesktop.OnIsInputBlocked -= OnInputBlockedFunc;
        }
        
        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerWeaponAttackState)) || 
                state.GetType().IsAssignableFrom(typeof(PlayerDefaultAttackState)))
            {
                UnblockInput(attackBlockInputType);
                isAttacking = false;
            }
        }
        
        public void HandleInput()
        {
            HandleHighlight();
            
            if (!isAttacking &&
                Input.GetMouseButtonUp(attackMouseButton) && 
                isCanUseControl(InputType.Attack) &&
                !IsPointerOverUIObject())
            {
                TriggerAttack();
            }
            else if (!isAttacking &&
                     Input.GetMouseButtonDown(specialActionMouseButton) && 
                     isCanUseControl(InputType.SpecialAction))
            {
                TriggerSpecialAction();
            }
            else if (Input.GetMouseButtonUp(specialActionMouseButton))
            {
                ExitSpecialAction();
            }
        }
        
        public void ClearSelectedObject()
        {
            selectedObject?.HideInformation();
            selectedRenderer?.UnSelectedObject();
            selectedObject = null;
        }
        
        private void HandleHighlight()
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
        
        private void TriggerAttack()
        {
            isAttacking = true;
            BlockInput(attackBlockInputType);
            characterSwitchAttack.ExitOtherStates();
            //characterSwitchAttack.SetState();
            characterControlDesktop.ClearHotkeys();
        }
        
        private void TriggerSelectObject()
        {
            if (tryGetHitPosition<IClickableObject>(out GameObject hitObject, Layers.EVERYTHING_LAYER))
            {
                var clickableObject = hitObject.GetComponent<IClickableObject>();
                clickableObject.UpdateInformation();
                if (selectedObject == null || selectedObject != clickableObject)
                {
                    selectedRenderer?.UnSelectedObject();
                    
                    selectedObject = clickableObject;
                    selectedObject.ShowInformation();
                    selectedRenderer = hitObject.GetComponent<UnitRenderer>();
                    selectedRenderer.SelectedObject();
                }
                else
                {
                    characterControlDesktop.ClearHotkeys();
                }
            }
        }

        private void TriggerSpecialAction()
        {
            InitializePlayerSpecialActionState();
            BlockInput(specialBlockInputType);
            stateMachine.ExitOtherStates(typeof(PlayerSpecialActionState));
            characterControlDesktop.ClearHotkeys();
        }
        
        private void ExitSpecialAction()
        {
            stateMachine.ExitCategory(StateCategory.Action, null, true);
            UnblockInput(specialBlockInputType);
            characterControlDesktop.ClearHotkeys();
        }
    }
}