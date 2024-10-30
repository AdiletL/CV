using Cysharp.Threading.Tasks;
using UnityEngine;

public class RewardSpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private PlatformSpawner platformSpawner;

    public Reward FinishReward {get; private set;}
    
    public async void Initialize()
    {
        
    }

    public void SetSpawners(PlatformSpawner platformSpawner)
    {
        this.platformSpawner = platformSpawner;
    }
    public async UniTask Execute()
    {
        SpawnGameObject();
    }

    private void SpawnGameObject()
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
