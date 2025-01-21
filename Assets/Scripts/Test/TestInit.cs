using System;
using Cysharp.Threading.Tasks;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class TestInit : MonoBehaviour
{
    [Inject] private DiContainer diContainer;
    
    [SerializeField] private AssetReference testPlayerPrefab;
    
    private GameUnits gameUnits;

    private TestPlayer _playerController;
    
    private async void Start()
    {
        await ASD();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public async UniTask ASD()
    {
        Debug.Log(diContainer);
        gameUnits = new GameUnits();
        diContainer.Bind<GameUnits>().FromInstance(gameUnits).AsSingle();
        
        var testPlayer = await Addressables.InstantiateAsync(testPlayerPrefab);
        _playerController = testPlayer.GetComponent<TestPlayer>();
        diContainer.Inject(_playerController);
        diContainer.Bind<TestPlayer>().FromInstance(_playerController).AsSingle();
        gameUnits.AddUnits(_playerController.gameObject);
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerController.Appear();
        }
    }
}
