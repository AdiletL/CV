using System;
using Gameplay.Spawner;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.Manager
{
    public class SpawnerManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private GameObject characterSpawnerPrefab;
        [SerializeField] private GameObject platformSpawnerPrefab;
        [SerializeField] private GameObject rewardSpawnerPrefab;
        [SerializeField] private GameObject trapSpawnerPrefab;

        private CharacterSpawner characterSpawner;
        private PlatformSpawner platformSpawner;
        private RewardSpawner rewardSpawner;
        private TrapSpawner trapSpawner;


        public void Initialize()
        {
            InstantiateSpawners();

            platformSpawner.Initialize();
            rewardSpawner.Initialize();
            characterSpawner.Initialize();
            trapSpawner.Initialize();
        }

        private void InstantiateSpawners()
        {
            platformSpawner =
                diContainer.InstantiatePrefabForComponent<PlatformSpawner>(platformSpawnerPrefab, transform);

            rewardSpawner = diContainer.InstantiatePrefabForComponent<RewardSpawner>(rewardSpawnerPrefab, transform);
            rewardSpawner.SetSpawners(platformSpawner);

            characterSpawner =
                diContainer.InstantiatePrefabForComponent<CharacterSpawner>(characterSpawnerPrefab, transform);
            characterSpawner.SetSpawners(platformSpawner, rewardSpawner);
            
            trapSpawner =  diContainer.InstantiatePrefabForComponent<TrapSpawner>(trapSpawnerPrefab, transform);
            trapSpawner.SetSpawners(platformSpawner);
        }

        private void Start()
        {
            StartSpawn();
        }

        private async void StartSpawn()
        {
            await platformSpawner.Execute();
            await rewardSpawner.Execute();
            await characterSpawner.Execute();
            await trapSpawner.Execute();
        }
    }
}
