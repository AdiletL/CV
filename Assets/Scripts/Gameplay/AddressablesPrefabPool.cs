using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressablesPrefabPool : IPunPrefabPool
{
    private readonly Dictionary<string, GameObject> prefabCache = new ();

    // Метод для создания объекта
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (!prefabCache.TryGetValue(prefabId, out GameObject prefab))
        {
            // Загружаем префаб через Addressables
            prefab = Addressables.LoadAssetAsync<GameObject>(prefabId).WaitForCompletion();
            if (prefab != null)
            {
                prefabCache[prefabId] = prefab;
            }
            else
            {
                Debug.LogError($"Prefab {prefabId} not found in Addressables!");
                return null;
            }
        }

        // Создаем экземпляр объекта, но не активируем его
        GameObject newObject = Object.Instantiate(prefab);
        if(position != Vector3.zero) newObject.transform.position = position;
        if(rotation != Quaternion.identity) newObject.transform.rotation = rotation;
        newObject.SetActive(false);  // Make sure the object is inactive when returned
        
        if(newObject.GetComponent<PhotonView>() == null)
            newObject.AddComponent<PhotonView>();
        
        return newObject;
    }

    

    // Метод для удаления объекта
    public void Destroy(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}