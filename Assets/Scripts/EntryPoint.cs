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
        
        var bootstrap = GameObject.FindFirstObjectByType<Bootstrap>();
        if (bootstrap == null)
        {
            Debug.LogError($"Bootstrap not found in scene {scene.name}");
            return;
        }
        
        bootstrap.Initialize();
        bootstrap.TransitionToScene(nextSceneName);
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    /*private async void Initialize()
    {
        await RunGame();
    }

    private async UniTask RunGame()
    {
#if UNITY_EDITOR
        var sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == BOOTSTRAP)
        {
            await LoadAndStartGamePlayAsync();
            return;
        }
        else if (sceneName == LABORATORY)
        {
            await LoadAndStartMenuAsync();
            return;
        }
        else if (sceneName == GAMEPLAY)
        {
            await LoadAndStartLevelBuildEditorAsync();
            return;
        }
#endif
        await LoadAndStartMenuAsync();
    }*/
}
