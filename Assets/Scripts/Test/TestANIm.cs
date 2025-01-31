using Photon.Pun;
using UnityEngine;

namespace Test
{
    public class TestANIm : MonoBehaviour
    {
        [SerializeField] private AnimationClip animationClip;

        private PhotonView photonView;
        private void Start()
        {
            photonView = GetComponent<PhotonView>();
            photonView.RPC(nameof(TestAnimation), RpcTarget.All);
        }

        [PunRPC]
        private void TestAnimation()
        {
            Debug.Log("fff");
        }
    }
}