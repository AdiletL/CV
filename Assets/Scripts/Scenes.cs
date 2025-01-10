using UnityEngine.SceneManagement;

public static class Scenes
{
    public const string BOOTSTRAP_NAME = "Bootstrap";
    public const string LABORATORY_NAME = "Laboratory";
    public const string GAMEPLAY_NAME = "Gameplay";

    public static void TransitionToScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
