using System;
using Photon.Pun;
using UnityEngine;

namespace Gameplay.Unit
{
    public class UnitTransformSync : MonoBehaviour, IPunObservable
    {
        private PhotonView photonView;

        private Vector3 latestPosition;
        private Quaternion latestRotation;

        private void Start()
        {
            photonView = GetComponent<PhotonView>();
        }
        
        private void LateUpdate()
        {
            /*if (!photonView.IsMine)
            {
                transform.position = Vector3.Lerp(transform.position, latestPosition, Time.deltaTime * 250);
                transform.rotation = Quaternion.Lerp(transform.rotation, latestRotation, Time.deltaTime * 250);
            }*/
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            /*if (stream.IsWriting) // Если это локальный игрок
            {
                // Отправляем данные о текущем состоянии
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else // Если это удалённый игрок
            {
                latestPosition = (Vector3)stream.ReceiveNext();
                latestRotation = (Quaternion)stream.ReceiveNext();
            }*/
        }
    }
}