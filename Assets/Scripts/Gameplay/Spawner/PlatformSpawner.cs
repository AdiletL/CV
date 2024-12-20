using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlatformSpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private Vector2Int length;
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject blockPrefab;

    private Transform platformParent, blockParent;

    private readonly List<Platform> platforms = new(20);
    

    public async void Initialize()
    {
        var newParentPlatform = new GameObject("Platform Parent");
        newParentPlatform.transform.position = Vector3.zero;
        platformParent = newParentPlatform.transform;

        var newParentBlock = new GameObject("Block Parent");
        newParentBlock.transform.position = Vector3.zero;
        blockParent = newParentBlock.transform;
        await UniTask.WaitForEndOfFrame();
    }

    public async UniTask Execute()
    {
        if (!platformParent || !platformPrefab) return;

        SpawnGameObject();

        await UniTask.WaitForEndOfFrame();
    }

    private void SpawnGameObject()
    {
        for (var x = 0; x < length.y; x++)
        for (var y = 0; y < length.x; y++)
        {
            var newGameObject = Instantiate(platformPrefab, platformParent.transform);
            newGameObject.transform.localPosition = new Vector3(x, 0, y);
            var platform = newGameObject.GetComponent<Platform>();
            platform.SetCoordinates(new Vector2Int(x + 1, y + 1));
            platform.IsBlocked = Random.Range(0, 6) == 0;
            if (platform.IsBlocked)
            {
                var block = Instantiate(blockPrefab, blockParent);
                block.transform.position = platform.transform.position;
                platform.AddGameObject(block);
            }

            platforms.Add(platform);
        }
    }

    public GameObject GetFreePlatform(Vector3 currentPosition = new Vector3())
    {
        var copyPlatforms = new List<Platform>(platforms);
        var randomIndex = 0;
        foreach (var item in platforms)
        {
            randomIndex = Random.Range(0, copyPlatforms.Count);
            var platform = copyPlatforms[randomIndex];

            if (platform.IsFreeForSpawn() && platform.transform.position != currentPosition)
                return platform.gameObject;
            copyPlatforms.Remove(platform);
        }

        return null;
    }
}