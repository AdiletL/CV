using System;
using Photon.Pun;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerMoveSync : MonoBehaviour, IPunObservable
    {
        private PhotonView photonView;
        
        private Vector3 latestPosition;
        private Quaternion latestRotation;

        private void Start()
        {
            photonView = GetComponent<PhotonView>();
            photonView.ObservedComponents.Add(this);
        }

        private void LateUpdate()
        {
            if (!photonView.IsMine) // Если это не локальный игрок
            {
                // Лерпаем позицию и поворот для плавного движения
                transform.position = Vector3.Lerp(transform.position, latestPosition, Time.deltaTime * 100);
                transform.rotation = Quaternion.Lerp(transform.rotation, latestRotation, Time.deltaTime * 100);
            }
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) // Если это локальный игрок
            {
                // Отправляем данные о текущем состоянии
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else // Если это удалённый игрок
            {
                latestPosition = (Vector3)stream.ReceiveNext();
                latestRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}