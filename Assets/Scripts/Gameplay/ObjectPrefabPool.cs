using Photon.Pun;
using UnityEngine;

namespace Gameplay
{
    public class ObjectPrefabPool : IPunPrefabPool
    {
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            GameObject newObject = new GameObject(prefabId);
            if(position != Vector3.zero) newObject.transform.position = position;
            if(rotation != Quaternion.identity) newObject.transform.rotation = rotation;
            newObject.SetActive(false);  // Make sure the object is inactive when returned
        
            if(PhotonNetwork.IsConnected && newObject.GetComponent<PhotonView>() == null)
                newObject.AddComponent<PhotonView>();
            
            return newObject;
        }

        public void Destroy(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }
    }
}