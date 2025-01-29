using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TextMeshProUGUI statusTxt;

    [Inject] private DiContainer diContainer;
    private void Start()
    {
        Caching.ClearCache();
        PhotonNetwork.ConnectUsingSettings();
        statusTxt.text = "Connecting...";
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        statusTxt.text = "Connected";
    }

    public override void OnJoinedLobby()
    {
        statusTxt.text = "Joined lobby";
    }

    public void CreateRoom()
    {
        string roomName = roomNameInputField.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions(){MaxPlayers = 4});
            statusTxt.text = "Creating room...";
        }
    }

    public void JoinRoom()
    {
        string roomName = roomNameInputField.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRoom(roomName);
            statusTxt.text = "Joined room...";
        }
    }

    public override void OnJoinedRoom()
    {
        statusTxt.text = "Joined room: " + PhotonNetwork.CurrentRoom.Name;
        Scenes.TransitionToScene(Scenes.BOOTSTRAP_NAME);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusTxt.text = "Failed to create room: " + message;
    }
}
