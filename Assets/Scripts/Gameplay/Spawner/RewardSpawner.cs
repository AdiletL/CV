using Cysharp.Threading.Tasks;
using UnityEngine;

public class RewardSpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private GameObject chestPrefab;

    private PlatformSpawner platformSpawner;
    public Reward FinishReward {get; private set;}
    
    public async void Initialize()
    {
        await UniTask.WaitForEndOfFrame();
    }

    public void SetSpawners(PlatformSpawner platformSpawner)
    {
        this.platformSpawner = platformSpawner;
    }
    public async UniTask Execute()
    {
        SpawnGameObject();
        
        await UniTask.WaitForEndOfFrame();
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

    public void ChangePositionReward()
    {
        FinishReward.transform.position = platformSpawner.GetFreePlatform().transform.position;
    }
}
