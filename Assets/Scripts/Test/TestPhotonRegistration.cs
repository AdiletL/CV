using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TestPhotonRegistration : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _testAnimation1;
    [SerializeField] private GameObject _testAnimation2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        PhotonNetwork.PrefabPool = new DefaultPool();
        Caching.ClearCache();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnJoinedLobby()
    {
        PhotonNetwork.CreateRoom("roomName", new RoomOptions(){MaxPlayers = 4});
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        PhotonNetwork.Instantiate(_testAnimation1.name, Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate(_testAnimation2.name, Vector3.zero, Quaternion.identity);
    }
}
