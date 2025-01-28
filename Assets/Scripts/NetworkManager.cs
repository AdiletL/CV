using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player other)
    {
        if (other == null)
        {
            Debug.LogError("OnPlayerEnteredRoom: Player is null.");
            return;
        }

        Debug.LogFormat("OnPlayerEnteredRoom: {0} joined the room.", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom: Master Client detected. Loading arena...");
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        if (other == null)
        {
            Debug.LogError("OnPlayerLeftRoom: Player is null.");
            return;
        }

        Debug.LogFormat("OnPlayerLeftRoom: {0} left the room.", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom: Master Client detected. Loading arena...");
        }
    }

    #endregion
}