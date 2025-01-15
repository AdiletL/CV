using System;
using Gameplay.Spawner;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.Manager
{
    public class GameManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private PoolManager poolManagerPrefab;
        [SerializeField] private LevelManager levelManagerPrefab;
        [SerializeField] private DamagePopUpSpawner damagePopUpSpawnerPrefab;
        [SerializeField] private PlayerController playerPrefab;
        
        private LevelManager levelManager;
        private PoolManager poolManager;
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
            Invoke(nameof(StartGame), .01f);
            // Отписываемся от события, чтобы избежать многократных вызовов
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        public void Initialize()
        {
            Debug.Log("Initializing Game Manager");
            InstantiateLevelManager();
            InstantiatePoolManager(); 
            InstantiateDamagePopUpSpawner();
        }

        private void InstantiateLevelManager()
        {
            levelManager = diContainer.InstantiatePrefabForComponent<LevelManager>(levelManagerPrefab);
            diContainer.Inject(levelManager);
            diContainer.Bind<LevelManager>().FromInstance(levelManager).AsSingle();
            levelManager.transform.SetParent(transform);
            levelManager.Initialize();
        }

        private void InstantiatePoolManager()
        {
            poolManager = diContainer.InstantiatePrefabForComponent<PoolManager>(poolManagerPrefab);
            diContainer.Inject(poolManager);
            diContainer.Bind<IPoolable>().FromInstance(poolManager).AsSingle();
            poolManager.transform.SetParent(transform);
            poolManager.Initialize();
        }
        
        private void InstantiateDamagePopUpSpawner()
        {
            damagePopUpSpawner = diContainer.InstantiatePrefabForComponent<DamagePopUpSpawner>(damagePopUpSpawnerPrefab);
            diContainer.Inject(damagePopUpSpawner);
            diContainer.Bind<DamagePopUpSpawner>().FromInstance(damagePopUpSpawner).AsSingle();
            damagePopUpSpawner.transform.SetParent(transform);
            damagePopUpSpawner.Initialize();
        }
        
        
        private void StartGame()
        {
            levelManager.StartLevel(playerPrefab);
        }
    }
}