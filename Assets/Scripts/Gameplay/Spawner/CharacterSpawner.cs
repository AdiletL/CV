using System;
using Character;
using Character.Enemy;
using Character.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] enemyPrefabs;
    
    private PlatformSpawner platformSpawner;
    private RewardSpawner rewardSpawner;
    
    private PlayerController playerCharacter;

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
        var freePlatform = platformSpawner.GetFreePlatform();
        if (freePlatform != null)
        {
            var playerGameObject = Instantiate(playerPrefab);
            playerGameObject.transform.position = freePlatform.transform.position;
            var player = playerGameObject.GetComponent<PlayerController>();
            player.SetFinishTarget(rewardSpawner.FinishReward.gameObject);
            player.Initialize();
            player.GetState<PlayerIdleState>().OnFinishedToTarget += OnFinishedToTargetPlayer;
            playerCharacter = player;
            
            freePlatform.GetComponent<Platform>().AddGameObject(playerGameObject);
        }
    }

    private void SpawnEnemies()
    {
        foreach (var VARIABLE in enemyPrefabs)
        {
            var freePlatform = platformSpawner.GetFreePlatform();
            if (freePlatform != null)
            {
                var enemyGameObject = Instantiate(VARIABLE);
                enemyGameObject.transform.position = freePlatform.transform.position;
                var enemy = enemyGameObject.GetComponent<EnemyController>();
                enemy.SetStartPlatform(freePlatform.GetComponent<Platform>());
                enemy.SetEndPlatform(platformSpawner.GetFreePlatform().GetComponent<Platform>());
                enemy.Initialize();
                //enemy.SetTarget()
                freePlatform.GetComponent<Platform>().AddGameObject(enemyGameObject);
            }
        }
    }

    private void OnFinishedToTargetPlayer()
    {
        rewardSpawner.ChangePositionReward();
        playerCharacter.SetFinishTarget(rewardSpawner.FinishReward.gameObject);
    }
}