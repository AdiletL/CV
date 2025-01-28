using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private AssetReference gameInstallerPrefab;
    
    public string nextScene;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Not Master Client, skipping initialization.");
            return;
        }
        var newGameObject = PhotonNetwork.Instantiate(gameInstallerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
        GetComponent<PhotonView>().RPC(nameof(Initialize), RpcTarget.AllBuffered, newGameObject.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    private void Initialize(int viewID)
    {
        var newGameObject = PhotonView.Find(viewID).gameObject;
        newGameObject.GetComponent<GameInstaller>().nextScene = Scenes.GAMEPLAY_NAME;
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
            var gameInstaller = GameObject.FindObjectOfType<GameInstaller>();
            var gameManager = GameObject.FindObjectOfType<GameManager>();
            if (bootstrap != null)
                GameObject.DestroyImmediate(bootstrap.gameObject);
            if (gameInstaller != null)
                GameObject.DestroyImmediate(gameInstaller.gameObject);
            if (gameManager != null)
                GameObject.DestroyImmediate(gameManager.gameObject);
        }
    }
}
#endif

