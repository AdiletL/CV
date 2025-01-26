using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Skill;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.Manager
{
    public class GameManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private AssetReferenceGameObject poolManagerPrefab;
        [SerializeField] private AssetReferenceGameObject levelManagerPrefab;
        [SerializeField] private AssetReferenceGameObject uiManagerPrefab;
        [SerializeField] private AssetReference so_SkillContainer;
        [SerializeField] private AssetReference so_GameConfig;
        
        private LevelManager levelManager;
        private IPoolableObject poolManager;
        private UIManager uiManager;
        private GameUnits gameUnits;
        private ExperienceSystem experienceSystem;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            // Подписываемся на событие загрузки сцены
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private async void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Bootstrap") return;

            await Initialize();
            // Отписываемся от события, чтобы избежать многократных вызовов
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public async UniTask Initialize()
        {
            await LoadAndBindAsset<SO_SkillContainer>(so_SkillContainer);
            
            var gameConfig = await LoadAndBindAsset<SO_GameConfig>(so_GameConfig);
            await LoadAndBindAsset<SO_GameUIColor>(gameConfig.SO_GameUIColor);
            await LoadAndBindAsset<SO_GameHotkeys>(gameConfig.SO_GameHotkeys);
            
            gameUnits = new GameUnits();
            diContainer.Inject(gameUnits);
            diContainer.Bind<GameUnits>().FromInstance(gameUnits).AsSingle();
            
            experienceSystem = new ExperienceSystem();
            diContainer.Inject(experienceSystem);
            diContainer.Bind<ExperienceSystem>().FromInstance(experienceSystem).AsSingle();
            
            await UniTask.WhenAll(
                InstantiatePoolManager(),
                InstantiateUIManager(),
                InstantiateLevelManager()
            );

            Debug.Log("Initializing: " + name);
            await UniTask.WaitForEndOfFrame();
            
            await StartGame();
        }

        private async UniTask<TInterface> InstantiateAndBind<TInterface, TConcrete>(AssetReference prefab)
            where TInterface : class
            where TConcrete : MonoBehaviour, TInterface
        {
            var result = PhotonNetwork.Instantiate(prefab.AssetGUID, Vector3.zero, Quaternion.identity);
            var instance = result.GetComponent<TConcrete>();
            diContainer.Inject(instance);
            diContainer.Bind<TInterface>().FromInstance(instance).AsSingle();
            result.transform.SetParent(transform);
            return instance;
        }

        private async UniTask<T> LoadAndBindAsset<T>(AssetReference asset) where T : ScriptableObject
        {
            var result = await Addressables.LoadAssetAsync<T>(asset);
            diContainer.Bind<T>().FromInstance(result).AsSingle();
            return result;
        }

        private async UniTask InstantiateLevelManager()
        {
            levelManager = await InstantiateAndBind<LevelManager, LevelManager>(levelManagerPrefab);
            await levelManager.Initialize();
        }

        private async UniTask InstantiatePoolManager()
        {
            poolManager = await InstantiateAndBind<IPoolableObject, PoolManager>(poolManagerPrefab);
            await poolManager.Initialize();
        }
        
        private async UniTask InstantiateUIManager()
        {
            uiManager = await InstantiateAndBind<UIManager, UIManager>(uiManagerPrefab);
            await uiManager.Initialize();
        }

        private async UniTask StartGame()
        {
            levelManager.StartLevelTest();
        }
    }
}
