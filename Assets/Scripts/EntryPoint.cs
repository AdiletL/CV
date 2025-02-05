using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPoint
{
    private string nextSceneName;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void AutostartGame()
    {
        var entryPoint = new EntryPoint();
        entryPoint.Initialize();
    }

    private void Initialize()
    {
        RunGame();
    }

    private void RunGame()
    {
        PhotonNetwork.PrefabPool = new AddressablesPrefabPool();
        SceneManager.sceneLoaded += OnSceneLoaded;
#if UNITY_EDITOR
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == Scenes.LABORATORY_NAME || sceneName == Scenes.GAMEPLAY_NAME)
        {
            nextSceneName = sceneName;
            Debug.Log("Current scene: " + sceneName);
            SceneManager.LoadSceneAsync(Scenes.LOBBY_NAME, LoadSceneMode.Single);
        }
        else if(sceneName == Scenes.LOBBY_NAME)
        {
            nextSceneName = Scenes.GAMEPLAY_NAME;
        }
        return;
#endif
        nextSceneName = Scenes.GAMEPLAY_NAME;
    }
    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name != Scenes.BOOTSTRAP_NAME) return;

        BoostrapLoad();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void BoostrapLoad()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        var bootstrap = PhotonNetwork.Instantiate("Bootstrap", Vector3.zero, Quaternion.identity, 0);
        bootstrap.GetComponent<Bootstrap>().nextScene = nextSceneName;
    }
}
