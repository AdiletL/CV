using Cysharp.Threading.Tasks;
using Unit.Reward;
using UnityEngine;
using Zenject;

public class RewardSpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private GameObject chestPrefab;

    private GameUnits gameUnits;
    private PlatformSpawner platformSpawner;
    private ChestController rewardController;

    [Inject]
    public void Construct(GameUnits gameUnits)
    {
        this.gameUnits = gameUnits;
    }
    
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
        var newPlatform = platformSpawner.GetFreePlace();
        if (newPlatform != null)
        {
            var newGameObject = Instantiate(chestPrefab);
            newGameObject.transform.position = newPlatform.transform.position;
            rewardController = newGameObject.GetComponent<ChestController>();
            rewardController.OnChestOpen += OnChestOpen;
            gameUnits.AddUnits(rewardController);
        }
    }

    private void OnChestOpen()
    {
        rewardController.transform.position = platformSpawner.GetFreePlace().transform.position;
    }
    
}
