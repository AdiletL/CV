using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private AssetReference gameInstallerPrefab;
    
    [HideInInspector] public string nextScene;

    private void Start()
    {
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 120;
        
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Not Master Client, skipping initialization.");
            return;
        }
        DontDestroyOnLoad(gameObject);
        GetComponent<PhotonView>().RPC(nameof(Initialize), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void Initialize()
    {
        Scenes.TransitionToScene(nextScene);
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
            var bootstrap = GameObject.FindObjectOfType<Bootstrap>();
            if (bootstrap != null)
                GameObject.DestroyImmediate(bootstrap.gameObject);
        }
    }
}
#endif

