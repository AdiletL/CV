using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TextMeshProUGUI statusTxt;

    private bool isOfflineMode = false; // Флаг оффлайн-режима

    private void Start()
    {
        Caching.ClearCache();

            ActivateOfflineMode();
            return;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
        }

        PhotonNetwork.ConnectUsingSettings();
        statusTxt.text = "Connecting...";
    }

    // Включение оффлайн-режима вручную
    public void ActivateOfflineMode()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // Отключаемся от сервера перед оффлайном
        }

        isOfflineMode = true;
        PhotonNetwork.OfflineMode = true;
        statusTxt.text = "Offline Mode Activated";
    }

    public override void OnConnectedToMaster()
    {
        if (!isOfflineMode)
        {
            PhotonNetwork.JoinLobby();
            statusTxt.text = "Connected to Master";
        }
    }

    public override void OnJoinedLobby()
    {
        statusTxt.text = "Joined lobby";
    }

    public void CreateRoom()
    {
        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName)) return;

        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.CreateRoom(roomName);
            statusTxt.text = "Offline Room Created";
        }
        else if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 });
            statusTxt.text = "Creating room...";
        }
        else
        {
            statusTxt.text = "Not connected!";
        }
    }

    public void JoinRoom()
    {
        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName)) return;

        if (PhotonNetwork.OfflineMode)
        {
            statusTxt.text = "Offline Mode - No need to join";
            OnJoinedRoom(); // Эмулируем вход в комнату
        }
        else if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
            statusTxt.text = "Joining room...";
        }
        else
        {
            statusTxt.text = "Not connected!";
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
