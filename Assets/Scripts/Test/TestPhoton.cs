using System;
using Photon.Pun;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

public class TestPhoton : MonoBehaviourPun
{
    [SerializeField] private PlayerController playerPrefab;
    [FormerlySerializedAs("customPrefabPool")] [SerializeField] private AddressablesPrefabPool addressablesPrefabPool;
    [SerializeField] private AssetReference assetReference;
    private void Start()
    {
        photonView.RPC("FDS", RpcTarget.All);
    }

    [PunRPC]
    private void FDS()
    {
        Debug.Log("FDS");
        addressablesPrefabPool.Instantiate(assetReference.AssetGUID, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>().Initialize();
    }
}
