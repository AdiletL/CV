using System;
using System.Collections.Generic;
using Gameplay.Common;
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

        
        private PlayerStateFactory playerStateFactory;
        private PlayerBlockInput playerBlockInput;
        private CharacterControlDesktop playerControlDesktop;
        private StateMachine stateMachine;
        
        private IClickableObject selectedObject;
        private UnitRenderer selectedRenderer;
        private UnitRenderer highlightedRenderer;
        private RaycastHit[] hits = new RaycastHit[5];

        private InputType attackBlockInputType;
        private InputType specialBlockInputType;
        
        private int selectObjectMousButton, attackMouseButton, specialActionMouseButton;
        private int hitRayOnObjectCount, hitsCount;
        
        private const float COOLDOWN_HIGHLIGHT_OBJECT = .2f;
        private float countCooldownHighlighObject;
        
        private bool isAttacking;
        private bool isSpecialAction;
        
        ~PlayerMouseInputHandler()
        {
            UnSubscribeEvent();
        }
        
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterControlDesktop(CharacterControlDesktop characterControlDesktop) => this.playerControlDesktop = characterControlDesktop;
        public void SetPlayerStateFactory(PlayerStateFactory playerStateFactory) => this.playerStateFactory = playerStateFactory;
        public void SetPlayerBlockInput(PlayerBlockInput playerBlockInput) => this.playerBlockInput = playerBlockInput;
        public void SetAttackBlockInput(InputType inputType) => attackBlockInputType = inputType;
        public void SetSpecialBlockInput(InputType inputType) => specialBlockInputType = inputType;
        
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
            attackMouseButton = so_GameHotkeys.AttackMouseButton;
            selectObjectMousButton = so_GameHotkeys.SelectObjectMouseButton;
            specialActionMouseButton = so_GameHotkeys.SpecialActionMouseButton;
            
            SubscribeEvent();
        }

        private void InitializePlayerSpecialActionState()
        {
            if(stateMachine.IsStateNotNull(typeof(PlayerSpecialActionState))) return;
            var state = (PlayerSpecialActionState)playerStateFactory.CreateState(typeof(PlayerSpecialActionState));
            diContainer.Inject(state);
            state.Initialize();
            stateMachine.AddStates(state);
        }

        private void SubscribeEvent()
        {
            stateMachine.OnExitCategory += OnExitCategory;
        }

        private void UnSubscribeEvent()
        {
            stateMachine.OnExitCategory -= OnExitCategory;
        }
        
        private void OnExitCategory(IState state)
        {
            if (typeof(CharacterAttackState).IsAssignableFrom(state.GetType()))
            {
                playerBlockInput.UnblockInput(attackBlockInputType);
                isAttacking = false;
            }
        }
        public void ClearSelectedObject()
        {
            selectedObject?.HideInformation();
            selectedRenderer?.UnSelectedObject();
            selectedObject = null;
        }
        
        public void HandleInput()
        {
            HandleHighlight();

            if (!isAttacking &&
                Input.GetMouseButtonDown(attackMouseButton) && 
                !playerBlockInput.IsInputBlocked(InputType.Attack) &&
                !CheckInputOnUI.IsPointerOverUIObject())
            {
                TriggerAttack();
            }
            else if (!isSpecialAction && Input.GetMouseButtonDown(specialActionMouseButton) && 
                     !playerBlockInput.IsInputBlocked(InputType.SpecialAction) && 
                     !CheckInputOnUI.IsPointerOverUIObject())
            {
                TriggerSpecialAction();
            }
            else if (isSpecialAction && Input.GetMouseButtonUp(specialActionMouseButton))
            {
                ExitSpecialAction();
            }
        }
        
        private void HandleHighlight()
        {
            countCooldownHighlighObject += Time.deltaTime;
            
            if(countCooldownHighlighObject < COOLDOWN_HIGHLIGHT_OBJECT) return;
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
            playerBlockInput.BlockInput(attackBlockInputType);
            stateMachine.ExitOtherStates(typeof(CharacterAttackState));
            playerControlDesktop.ClearHotkeys();
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
                    playerControlDesktop.ClearHotkeys();
                }
            }
        }

        private void TriggerSpecialAction()
        {
            InitializePlayerSpecialActionState();
            playerBlockInput.BlockInput(specialBlockInputType);
            stateMachine.ExitOtherStates(typeof(PlayerSpecialActionState));
            playerControlDesktop.ClearHotkeys();
            isSpecialAction = true;
        }
        
        private void ExitSpecialAction()
        {
            stateMachine.ExitCategory(StateCategory.Action, null, true);
            playerBlockInput.UnblockInput(specialBlockInputType);
            playerControlDesktop.ClearHotkeys();
            isSpecialAction = false;
        }
    }

    public class PlayerMouseInputHandlerBuilder
    {
        private PlayerMouseInputHandler handler = new ();

        public PlayerMouseInputHandlerBuilder SetStateMachine(StateMachine stateMachine)
        {
            handler.SetStateMachine(stateMachine);
            return this;
        }
        public PlayerMouseInputHandlerBuilder SetCharacterControlDesktop(
            CharacterControlDesktop characterControlDesktop)
        {
            handler.SetCharacterControlDesktop(characterControlDesktop);
            return this;
        }
        public PlayerMouseInputHandlerBuilder SetPlayerStateFactory(PlayerStateFactory playerStateFactory)
        {
            handler.SetPlayerStateFactory(playerStateFactory);
            return this;
        }
        public PlayerMouseInputHandlerBuilder SetPlayerBlockInput(PlayerBlockInput playerBlockInput)
        {
            handler.SetPlayerBlockInput(playerBlockInput);
            return this;
        }
        public PlayerMouseInputHandlerBuilder SetAttackBlockInput(InputType inputType)
        {
            handler.SetAttackBlockInput(inputType);
            return this;
        }
        public PlayerMouseInputHandlerBuilder SetSpecialBlockInput(InputType inputType)
        {
            handler.SetSpecialBlockInput(inputType);
            return this;
        }

        public PlayerMouseInputHandler Build()
        {
            return handler;
        }
    }
}