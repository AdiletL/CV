using Cysharp.Threading.Tasks;
using Gameplay.Spawner;
using Gameplay.UI.ScreenSpace;
using Gameplay.UI.ScreenSpace.ContextMenu;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Manager
{
    public class UIManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private AssetReferenceGameObject mainCanvasPrefab;
        [SerializeField] private AssetReferenceGameObject uiCastTimerPrefab;
        [SerializeField] private AssetReferenceGameObject uiContextMenuPrefab;
        [SerializeField] private AssetReferenceGameObject uiAbilityTooltipPrefab;
        [SerializeField] private AssetReferenceGameObject uiItemTooltipPrefab;
        
        private GameObject mainCanvasGameObject;
        
        public void Initialize()
        {
            InitializeMainCanvas();
            InitializeUICastTimer();
            InitializeUIContextMenu();
            InitializeUIAbilityTooltipView();

            var damagePopUpSpawner = new DamagePopUpSpawner();
            diContainer.Inject(damagePopUpSpawner);
            diContainer.Bind(damagePopUpSpawner.GetType()).FromInstance(damagePopUpSpawner).AsSingle();
            
            var healPopUpSpawner = new HealPopUpSpawner();
            diContainer.Inject(healPopUpSpawner);
            diContainer.Bind(healPopUpSpawner.GetType()).FromInstance(healPopUpSpawner);
            
            var protectionPopUpSpawner = new ProtectionPopUpSpawner();
            diContainer.Inject(protectionPopUpSpawner);
            diContainer.Bind(protectionPopUpSpawner.GetType()).FromInstance(protectionPopUpSpawner).AsSingle();
            
            var evasionPopUpSpawner = new EvasionPopUpSpawner();
            diContainer.Inject(evasionPopUpSpawner);
            diContainer.Bind(evasionPopUpSpawner.GetType()).FromInstance(evasionPopUpSpawner).AsSingle();
            
            var criticalDamagePopUpSpawner = new CriticalDamagePopUpSpawner();
            diContainer.Inject(criticalDamagePopUpSpawner);
            diContainer.Bind(criticalDamagePopUpSpawner.GetType()).FromInstance(criticalDamagePopUpSpawner).AsSingle();
        }

        private void InitializeMainCanvas()
        {
            var loadGameObject = Addressables.LoadAssetAsync<GameObject>(mainCanvasPrefab).WaitForCompletion();
            mainCanvasGameObject = diContainer.InstantiatePrefab(loadGameObject);
        }

        private void InitializeUICastTimer()
        {
            var loadGameObject = Addressables.LoadAssetAsync<GameObject>(uiCastTimerPrefab).WaitForCompletion();
            var newGameObject = diContainer.InstantiatePrefab(loadGameObject);
            var uiCastTimer = newGameObject.GetComponent<UICastTimer>();
            diContainer.Bind(uiCastTimer.GetType()).FromInstance(uiCastTimer).AsSingle();
            uiCastTimer.Initialize();
        }
        
        private void InitializeUIContextMenu()
        {
            var loadGameObject = Addressables.LoadAssetAsync<GameObject>(uiContextMenuPrefab).WaitForCompletion();
            var newGameObject = diContainer.InstantiatePrefab(loadGameObject);
            var uiContextMenu = newGameObject.GetComponent<UIContextMenu>();
            diContainer.Bind(uiContextMenu.GetType()).FromInstance(uiContextMenu).AsSingle();
            newGameObject.transform.SetParent(mainCanvasGameObject.transform);
        }
        
        private void InitializeUIAbilityTooltipView()
        {
            var loadGameObject = Addressables.LoadAssetAsync<GameObject>(uiAbilityTooltipPrefab).WaitForCompletion();
            var newGameObject = diContainer.InstantiatePrefab(loadGameObject);
            var uiAbilityTooltipView = newGameObject.GetComponent<UITooltipView>();
            diContainer.Inject(uiAbilityTooltipView);
            diContainer.Bind(uiAbilityTooltipView.GetType()).FromInstance(uiAbilityTooltipView).AsSingle();
            uiAbilityTooltipView.Hide();
        }
    }
}