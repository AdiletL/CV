using Photon.Pun;
using UnityEngine;

public class TestAnimation
{
    
    private PhotonView photonView;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(PhotonView photonView, AnimationClip animationClip)
    {
        this.photonView = photonView;
        this.photonView.RPC(nameof(PlayAnimation), RpcTarget.All);
    }

    
    [PunRPC]
    private void PlayAnimation()
    {
        Debug.Log("asdf");
    }
}
