using Cysharp.Threading.Tasks;
using Gameplay.Spawner;
using ScriptableObjects.Gameplay.Skill;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay.Manager
{
    public class GameManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private AssetReference poolManagerPrefab;
        [SerializeField] private AssetReference levelManagerPrefab;
        [SerializeField] private AssetReference damagePopUpSpawnerPrefab;
        [SerializeField] private AssetReference playerPrefab;
        [SerializeField] private AssetReference so_SkillContainer;

        private LevelManager levelManager;
        private IPoolableObject poolManager; // Используем интерфейс
        private DamagePopUpSpawner damagePopUpSpawner;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            // Подписываемся на событие загрузки сцены
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Bootstrap") return;

            Initialize();
            // Отписываемся от события, чтобы избежать многократных вызовов
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public async void Initialize()
        {
            Debug.Log("Initializing: " + name);

            await UniTask.WhenAll(
                InstantiateLevelManager(),
                InstantiatePoolManager(),
                InstantiateDamagePopUpSpawner()
            );

            await LoadAndBindAsset<SO_SkillContainer>(so_SkillContainer);
            StartGame();
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
            levelManager.Initialize();
        }

        private async UniTask InstantiatePoolManager()
        {
            poolManager = await InstantiateAndBind<IPoolableObject, PoolManager>(poolManagerPrefab);
            poolManager.Initialize();
        }

        private async UniTask InstantiateDamagePopUpSpawner()
        {
            damagePopUpSpawner = await InstantiateAndBind<DamagePopUpSpawner, DamagePopUpSpawner>(damagePopUpSpawnerPrefab);
            damagePopUpSpawner.Initialize();
        }

        private async void StartGame()
        {
            levelManager.StartLevel(playerPrefab);
        }
    }
}