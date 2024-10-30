using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpawnerManager : MonoBehaviour, IManager
{
    [SerializeField] private CharacterSpawner characterSpawnerPrefab;
    [SerializeField] private PlatformSpawner platformSpawnerPrefab;
    [SerializeField] private RewardSpawner rewardSpawnerPrefab;
    [SerializeField] private TrapSpawner trapSpawnerPrefab;
    
    private CharacterSpawner characterSpawner;
    private PlatformSpawner platformSpawner;
    private RewardSpawner rewardSpawner;
    private TrapSpawner trapSpawner;

    private void Awake()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        InstantiateSpawners();
        
        platformSpawner.Initialize();
        rewardSpawner.Initialize();
        characterSpawner.Initialize();
    }

    private async void InstantiateSpawners()
    {
        platformSpawner = Instantiate(platformSpawnerPrefab, transform);
        rewardSpawner = Instantiate(rewardSpawnerPrefab, transform);
        rewardSpawner.SetSpawners(platformSpawner);
        characterSpawner = Instantiate(characterSpawnerPrefab, transform);
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
