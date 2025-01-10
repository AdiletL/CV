using System;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay.Manager
{
    public class GameManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private PoolManager poolManagerPrefab;
        [SerializeField] private LevelManager levelManagerPrefab;
        [SerializeField] private PlayerController playerPrefab;
        
        private LevelManager levelManager;
        private PoolManager poolManager;
        
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
        }
        

        private void InstantiateLevelManager()
        {
            levelManager = diContainer.InstantiatePrefabForComponent<LevelManager>(levelManagerPrefab);
            diContainer.BindInstance(levelManager).AsSingle();
            levelManager.transform.SetParent(transform);
            levelManager.Initialize();
        }

        private void InstantiatePoolManager()
        {
            poolManager = diContainer.InstantiatePrefabForComponent<PoolManager>(poolManagerPrefab);
            diContainer.BindInstance(poolManager).AsSingle();
            poolManager.transform.SetParent(transform);
            poolManager.Initialize();
        }

        private void StartGame()
        {
            levelManager.StartLevel(playerPrefab);
        }
    }
}