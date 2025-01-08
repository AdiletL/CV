using System;
using Unit.Character.Creep;
using Unit.Character.Player;
using Cysharp.Threading.Tasks;
using Unit.Cell;
using UnityEngine;
using Zenject;

public class CharacterSpawner : MonoBehaviour, ISpawner
{
    [Inject] private DiContainer diContainer;
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] enemyPrefabs;
    
    private GameUnits gameUnits;
    private PlatformSpawner platformSpawner;
    private RewardSpawner rewardSpawner;
    
    private PlayerController playerCharacter;
    

    [Inject]
    public void Construct(GameUnits gameUnits)
    {
        this.gameUnits = gameUnits;
    }

    public async void Initialize()
    {
        await UniTask.WaitForEndOfFrame();
    }

    public async UniTask Execute()
    {
        SpawnGameObject();
        await UniTask.WaitForEndOfFrame();
    }

    private void SpawnGameObject()
    {
        SpawnPlayer();
        SpawnEnemies();
    }

    public void SetSpawners(PlatformSpawner platformSpawner, RewardSpawner rewardSpawner)
    {
        this.platformSpawner = platformSpawner;
        this.rewardSpawner = rewardSpawner;
    }
    
    private void SpawnPlayer()
    {
        var platform = platformSpawner.GetFreePlace();
        if(!platform) return;
        
        var playerGameObject = diContainer.InstantiatePrefabForComponent<PlayerController>(playerPrefab);
        playerGameObject.transform.position = platform.transform.position;
        var player = playerGameObject.GetComponent<PlayerController>();
        player.Initialize();
        player.GetState<PlayerIdleState>().OnFinishedToTarget += OnFinishedToTargetPlayer;
        playerCharacter = player;
        
        gameUnits.AddUnits(player);
    }

    private void SpawnEnemies()
    {
        foreach (var VARIABLE in enemyPrefabs)
        {
            var platform = platformSpawner.GetFreePlace();
            if (!platform) return;
            
            var enemyGameObject = diContainer.InstantiatePrefabForComponent<CreepController>(VARIABLE);
            enemyGameObject.transform.position = platform.transform.position;
            var enemy = enemyGameObject.GetComponent<CreepController>();
            enemy.SetStart(platform.GetComponent<CellController>().transform);
            enemy.SetEnd(platformSpawner.GetFreePlace(platform.transform.position).GetComponent<CellController>().transform);
            enemy.Initialize();
            //enemy.SetTarget()
            gameUnits.AddUnits(enemy);
        }
    }

    private void OnFinishedToTargetPlayer()
    {
        //rewardSpawner.ChangePositionReward();
        //playerCharacter.SetFinishTarget(rewardSpawner.FinishReward.gameObject);
    }
}