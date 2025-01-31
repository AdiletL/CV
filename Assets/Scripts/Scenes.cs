using Photon.Pun;
using UnityEngine.SceneManagement;

public static class Scenes
{
    public const string BOOTSTRAP_NAME = "Bootstrap";
    public const string LABORATORY_NAME = "Laboratory";
    public const string GAMEPLAY_NAME = "Gameplay";
    public const string TEST_NAME = "Test";
    public const string LOBBY_NAME = "Lobby";

    public static void TransitionToScene(string sceneName)
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(sceneName);
        //SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
