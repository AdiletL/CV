using Cysharp.Threading.Tasks;
using Gameplay.Spawner;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Skill;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay.Manager
{
    public class GameManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private AssetReferenceGameObject poolManagerPrefab;
        [SerializeField] private AssetReferenceGameObject levelManagerPrefab;
        [SerializeField] private AssetReferenceGameObject damagePopUpSpawnerPrefab;
        [SerializeField] private AssetReference so_SkillContainer;
        [SerializeField] private AssetReference so_GameHotkey;

        private LevelManager levelManager;
        private IPoolableObject poolManager; // Используем интерфейс
        private DamagePopUpSpawner damagePopUpSpawner;

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
            Debug.Log("Initializing: " + name);

            await UniTask.WhenAll(
                InstantiateLevelManager(),
                InstantiatePoolManager(),
                InstantiateDamagePopUpSpawner()
            );

            await LoadAndBindAsset<SO_SkillContainer>(so_SkillContainer);
            await LoadAndBindAsset<SO_GameHotkey>(so_GameHotkey);
            await StartGame();
        }

        private async UniTask<TInterface> InstantiateAndBind<TInterface, TConcrete>(AssetReference prefab)
            where TInterface : class
            where TConcrete : MonoBehaviour, TInterface
        {
            var result = await Addressables.LoadAssetAsync<GameObject>(prefab);
            var instance = diContainer.InstantiatePrefabForComponent<TConcrete>(result);
            diContainer.Inject(instance);
            diContainer.Bind<TInterface>().FromInstance(instance).AsSingle();
            instance.transform.SetParent(transform);
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

        private async UniTask InstantiateDamagePopUpSpawner()
        {
            damagePopUpSpawner = await InstantiateAndBind<DamagePopUpSpawner, DamagePopUpSpawner>(damagePopUpSpawnerPrefab);
            await damagePopUpSpawner.Initialize();
        }

        private async UniTask StartGame()
        {
            await levelManager.StartLevel();
        }
    }
}