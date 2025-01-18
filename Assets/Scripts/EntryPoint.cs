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
#if UNITY_EDITOR
        SceneManager.sceneLoaded += OnSceneLoaded;
        var sceneName = SceneManager.GetActiveScene().name;

        nextSceneName = sceneName;
        if (sceneName == Scenes.LABORATORY_NAME || sceneName == Scenes.GAMEPLAY_NAME)
        {
            Debug.Log("Current scene: " + sceneName);
            SceneManager.LoadSceneAsync(Scenes.BOOTSTRAP_NAME, LoadSceneMode.Single);
        }
#endif
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
