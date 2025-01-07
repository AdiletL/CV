using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Manager
{
    public class GameManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private PoolManager poolManagerPrefab;
        [SerializeField] private LevelManager levelManagerPrefab;
        
        private LevelManager levelManager;
        private PoolManager poolManager;
        
        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            InstantiateLevelManager();
            InstantiatePoolManager();
        }
        

        private void InstantiateLevelManager()
        {
            levelManager = diContainer.InstantiatePrefabForComponent<LevelManager>(levelManagerPrefab);
            diContainer.BindInstance(levelManager).AsSingle();
            levelManager.Initialize();
        }

        private void InstantiatePoolManager()
        {
            poolManager = diContainer.InstantiatePrefabForComponent<PoolManager>(poolManagerPrefab);
            diContainer.BindInstance(poolManager).AsSingle();
            poolManager.Initialize();
        }

        private void Start()
        {
            levelManager.StartLevel();
        }
    }
}