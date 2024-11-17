using System;
using Cysharp.Threading.Tasks;
using Gameplay;
using UnityEngine;
using Zenject;

public class SpawnerManager : MonoBehaviour, IManager
{
    [Inject] private DiContainer diContainer;
    
    [SerializeField] private CharacterSpawner characterSpawnerPrefab;
    [SerializeField] private PlatformSpawner platformSpawnerPrefab;
    [SerializeField] private RewardSpawner rewardSpawnerPrefab;
    [SerializeField] private TrapSpawner trapSpawnerPrefab;
    
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
    }

    private void InstantiateSpawners()
    {
        platformSpawner = diContainer.InstantiatePrefabForComponent<PlatformSpawner>(platformSpawnerPrefab, transform);
        
        rewardSpawner = diContainer.InstantiatePrefabForComponent<RewardSpawner>(rewardSpawnerPrefab, transform);
        rewardSpawner.SetSpawners(platformSpawner);
        
        characterSpawner = diContainer.InstantiatePrefabForComponent<CharacterSpawner>(characterSpawnerPrefab, transform);
        characterSpawner.SetSpawners(platformSpawner, rewardSpawner);
        //trapSpawner = Instantiate(trapSpawnerPrefab, transform);
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
    }
}
