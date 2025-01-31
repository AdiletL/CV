using System;
using Photon.Pun;
using UnityEngine;

public abstract class TestAn : MonoBehaviour
{
    [SerializeField] private AnimationClip animationClip;

    private PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        photonView.RPC(nameof(TestAnimation), RpcTarget.All);
    }

    [PunRPC]
    protected void TestAnimation()
    {
        Debug.Log("dddd");
    }
}
