using System;
using System.Collections.Generic;
using ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerMouseInputHandler : IInputHandler
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeyse;
        
        private PlayerSkillInputHandler playerSkillInputHandler;
        private PlayerInventory playerInventory;
        private CharacterControlDesktop characterControlDesktop;
        private CharacterSwitchAttackState characterSwitchAttack;
        private StateMachine stateMachine;
        
        private IClickableObject selectedObject;
        private UnitRenderer selectedRenderer;
        private UnitRenderer highlightedRenderer;
        private RaycastHit[] hits = new RaycastHit[5];
        
        private int selectObjectMousButton, attackMouseButton;
        private int hitRayOnObjectCount, hitsCount;
        
        private const float cooldownHighlighObject = .2f;
        private float countCooldownHighlighObject;
        
        private bool isAttacking;
        
        private Dictionary<InputType, int> blockedInputs = new();
        
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void CharacterControlDesktop(CharacterControlDesktop characterControlDesktop) => this.characterControlDesktop = characterControlDesktop;
        public void SetPlayerSkillInputHandler(PlayerSkillInputHandler playerSkillInputHandler) => this.playerSkillInputHandler = playerSkillInputHandler;
        public void SetCharacterSwitchAttack(CharacterSwitchAttackState characterSwitchAttackState) =>
            this.characterSwitchAttack = characterSwitchAttackState;

        public PlayerMouseInputHandler(StateMachine stateMachine, CharacterControlDesktop characterControlDesktop,
            PlayerSkillInputHandler playerSkillInputHandler, CharacterSwitchAttackState characterSwitchAttackState,
            PlayerInventory playerInventory)
        {
            this.stateMachine = stateMachine;
            this.characterControlDesktop = characterControlDesktop;
            this.playerSkillInputHandler = playerSkillInputHandler;
            this.characterSwitchAttack = characterSwitchAttackState;
            this.playerInventory = playerInventory;
            
            stateMachine.OnExitCategory += OnExitCategory;
        }

        ~PlayerMouseInputHandler()
        {
            stateMachine.OnExitCategory -= OnExitCategory;
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
                if (flag == InputType.nothing || (input & flag) == 0) continue;

                if (blockedInputs.ContainsKey(flag) && blockedInputs[flag] > 0)
                    return true;
            }
            return false;
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

        
        public void Initialize()
        {
            attackMouseButton = so_GameHotkeyse.AttackMouseButton;
            selectObjectMousButton = so_GameHotkeyse.SelectObjectMouseButton;
        }

        private void OnExitCategory(Machine.IState state)
        {
            if (stateMachine.IsStateNotNull(typeof(PlayerWeaponAttackState)) || 
                stateMachine.IsStateNotNull(typeof(PlayerDefaultAttackState)))
            {
                isAttacking = false;
            }
        }
        
        public void HandleInput()
        {
            HandleHighlight();
            
            if (!isAttacking &&
                Input.GetMouseButtonDown(attackMouseButton) && 
                !characterControlDesktop.IsInputBlocked(InputType.attack) &&
                !playerSkillInputHandler.IsInputBlocked(InputType.attack) && 
                !playerInventory.IsInputBlocked(InputType.attack) && 
                !IsPointerOverUIObject())
            {
                TriggerAttack();
            }
            else if (Input.GetMouseButtonDown(selectObjectMousButton) && 
                     !characterControlDesktop.IsInputBlocked(InputType.selectObject) && 
                     !playerSkillInputHandler.IsInputBlocked(InputType.selectObject))
            {
                TriggerSelectObject();
            }
        }
        
        public void ClearSelectedObject()
        {
            selectedObject?.HideInformation();
            selectedRenderer?.UnSelectedObject();
            selectedObject = null;
            playerInventory.ClearSelectedItem();
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
            characterSwitchAttack.ExitOtherStates();
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
    }
}