using UnityEngine;

public class RewardSpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private PlatformSpawner platformSpawner;

    public Reward FinishReward {get; private set;}
    
    public void Initialize()
    {
        
    }

    public void SetSpawners(PlatformSpawner platformSpawner)
    {
        this.platformSpawner = platformSpawner;
    }
    public void Execute()
    {
        SpawnGameObject();
    }

    public void SpawnGameObject()
    {
        var freePlatform = platformSpawner.GetFreePlatform();
        if (freePlatform != null)
        {
            var newGameObject = Instantiate(chestPrefab);
            newGameObject.transform.position = freePlatform.transform.position;
            freePlatform.GetComponent<Platform>().AddGameObject(newGameObject);
            FinishReward = newGameObject.GetComponent<Reward>();
        }
    }

}
