using Character.Enemy;
using Characters.Player;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private PlatformSpawner platformSpawner;
    [SerializeField] private RewardSpawner rewardSpawner;

    public void Initialize()
    {
    }

    public void Execute()
    {
        SpawnGameObject();
    }

    public void SpawnGameObject()
    {
        SpawnPlayer();
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
            player.Initialize();
            player.SetTarget(rewardSpawner.FinishReward.gameObject);
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
                enemy.Initialize();
                freePlatform.GetComponent<Platform>().AddGameObject(enemyGameObject);
            }
        }
    }
}