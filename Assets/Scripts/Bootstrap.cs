using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private AssetReference gameInstallerPrefab;
    [SerializeField] private AssetReference gameManagerPrefab;
    [SerializeField] private AssetReference laboratoryManagerPrefab;

    private GameInstaller currentGameInstaller;

    public async UniTask Initialize()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        
        var newObject = PhotonNetwork.Instantiate(gameInstallerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
        currentGameInstaller = newObject.GetComponent<GameInstaller>();
    }

    public async UniTask TransitionToScene(string sceneName)
    {
        if(!PhotonNetwork.IsMasterClient) return;
        await CreateManagerAsync(sceneName);
        Scenes.TransitionToScene(sceneName);
    }

    private async UniTask CreateManagerAsync(string sceneName)
    {
        switch (sceneName)
        {
            case Scenes.GAMEPLAY_NAME:
                await InstantiateManagerAsyncA<Gameplay.Manager.GameManager>(gameManagerPrefab);
                break;
            case Scenes.LABORATORY_NAME:
                await InstantiateManagerAsyncA<Laboratory.Manager.LaboratoryManager>(laboratoryManagerPrefab);
                break;
            case Scenes.TEST_NAME:
                
                break;
            default:
                throw new ArgumentException($"Unknown scene name: {sceneName}");
        }
    }

    private async UniTask<T> InstantiateManagerAsyncA<T>(AssetReference prefabReference) where T : MonoBehaviour
    {
        var instance = currentGameInstaller.InstantiatePrefab(prefabReference).GetComponent<T>();
        return instance;
    }
}

#if UNITY_EDITOR
[InitializeOnLoad]
public static class PlayModeCleanup
{
    static PlayModeCleanup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            var gameInstaller = GameObject.FindObjectOfType<GameInstaller>();
            if (gameInstaller != null)
            {
                GameObject.DestroyImmediate(gameInstaller.gameObject);
            }
        }
    }
}
#endif