using System;
using Gameplay.UI.ScreenSpace;
using Gameplay.UI.ScreenSpace.Stats;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.Unit.Character
{
    public class CharacterPortrait : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private CharacterMainController characterMainController;
        [SerializeField] private AssetReferenceGameObject uiPortraitPrefab;
        [SerializeField] private AssetReferenceGameObject rangeAttackDisplayPrefab;
        
        private UITooltipEvent uiTooltipEvent;
        private MediatorStatsAndUI mediatorStatsAndUI;
        private GameObject uiPortrait;
        private RangeDisplay rangeAttack;
        

        public void Initialize(IStatsController statsController)
        {
            uiPortrait = Addressables.InstantiateAsync(uiPortraitPrefab).WaitForCompletion();
            
            var uiStats = uiPortrait.GetComponentInChildren<UIStats>();
            diContainer.Inject(uiStats);
            uiStats.Initialize();
            
            mediatorStatsAndUI = new MediatorStatsAndUI(statsController, uiStats);
            mediatorStatsAndUI.Initialize();
            
            uiTooltipEvent = uiPortrait.GetComponentInChildren<UITooltipEvent>();
            uiTooltipEvent.OnEnter += OnTooltipEnter;
            uiTooltipEvent.OnExit += OnTooltipExit;
        }

        private void OnTooltipEnter()
        {
            if (characterMainController.StateMachine.TryGetInterfaceImplementingClass(out IAttack attack))
            {
                CreateRangeAttack();
                rangeAttack.SetRange(attack.RangeAttackStat.CurrentValue);
                rangeAttack.ShowRange();
            }
        }

        private void OnTooltipExit()
        {
            rangeAttack?.HideRange();
        }

        private void CreateRangeAttack()
        {
            if (rangeAttack) return;
            var newGameObject = Addressables.InstantiateAsync(rangeAttackDisplayPrefab, 
                characterMainController.VisualParent.transform).WaitForCompletion();
            newGameObject.transform.localPosition = Vector3.zero;
            rangeAttack = newGameObject.GetComponent<RangeDisplay>();
        }
        
        private void OnDestroy()
        {
            uiTooltipEvent.OnEnter -= OnTooltipEnter;
            uiTooltipEvent.OnExit -= OnTooltipExit;
        }
    }
}