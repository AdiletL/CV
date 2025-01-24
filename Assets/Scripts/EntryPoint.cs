using Cysharp.Threading.Tasks;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
#if UNITY_EDITOR
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == Scenes.LABORATORY_NAME || sceneName == Scenes.GAMEPLAY_NAME)
        {
            nextSceneName = sceneName;
            Debug.Log("Current scene: " + sceneName);
            SceneManager.LoadSceneAsync(Scenes.BOOTSTRAP_NAME, LoadSceneMode.Single);
        }
        else if (sceneName == Scenes.BOOTSTRAP_NAME)
        {
            nextSceneName = Scenes.GAMEPLAY_NAME;
            SceneManager.LoadSceneAsync(Scenes.GAMEPLAY_NAME, LoadSceneMode.Single);
        }
        return;
#endif
        nextSceneName = Scenes.GAMEPLAY_NAME;
        SceneManager.LoadSceneAsync(Scenes.GAMEPLAY_NAME, LoadSceneMode.Single);
    }
    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name != Scenes.BOOTSTRAP_NAME) return;

        BoostrapLoad();
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private async void BoostrapLoad()
    {
        var bootstrap = GameObject.FindFirstObjectByType<Bootstrap>();
        if (bootstrap == null)
        {
            Debug.LogError($"Bootstrap not found in scene {SceneManager.GetActiveScene().name}");
            return;
        }
        
        await bootstrap.Initialize();
        await bootstrap.TransitionToScene(nextSceneName);
    }
}
