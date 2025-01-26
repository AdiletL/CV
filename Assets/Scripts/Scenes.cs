using Photon.Pun;
using UnityEngine.SceneManagement;

public static class Scenes
{
    public const string BOOTSTRAP_NAME = "Bootstrap";
    public const string LABORATORY_NAME = "Laboratory";
    public const string GAMEPLAY_NAME = "Gameplay";
    public const string TEST_NAME = "Test";

    public static void TransitionToScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
        //SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
