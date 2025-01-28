using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Manager
{
    public class PoolManager : MonoBehaviour, IManager, IPoolableObject
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private AssetReferenceGameObject[] poolPrefabReferences;
        [SerializeField] private int initialPoolSize = 5;

        private AssetReference suitablePrefab;
        private SemaphoreSlim semaphore = new(System.Environment.ProcessorCount / 2);
        
        public Transform poolParent { get; private set; }
        public List<GameObject> PoolObjects { get; private set; } = new();
        
        private PhotonView photonView;

        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
            if(!PhotonNetwork.IsMasterClient) return;
            
            photonView.RPC(nameof(CreatePoolParent), RpcTarget.AllBuffered);
            
            foreach (var prefabReference in poolPrefabReferences)
            {
                for (int i = 0; i < initialPoolSize; i++)
                {
                    var result =
                        PhotonNetwork.Instantiate(prefabReference.AssetGUID, Vector3.zero, Quaternion.identity);
                    photonView.RPC(nameof(InjectComponents), RpcTarget.AllBuffered, result.GetComponent<PhotonView>().ViewID);
                }
            }
        }

        [PunRPC]
        private void CreatePoolParent()
        {
            poolParent = new GameObject("PoolParent").transform;
        }

        [PunRPC]
        private void InjectComponents(int viewID)
        {
            var targetGameObject = PhotonView.Find(viewID).gameObject;
            var components = targetGameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
                diContainer.Inject(component);
            targetGameObject.transform.SetParent(poolParent);
            targetGameObject.SetActive(false);
        }

        [PunRPC]
        private void InjectComponent(int viewID)
        {
            var targetGameObject = PhotonView.Find(viewID).gameObject;
            var component = targetGameObject.GetComponent<MonoBehaviour>();
            diContainer.Inject(component);
        }
        
        public async UniTask<GameObject> GetObjectAsync<T>()
        {
            // Попытка найти объект в пуле
            var poolObject = GetPoolObjectFromList<T>();
            if (poolObject != null)
                return poolObject;

            // Если не найден в пуле, загружаем из префабов
            return await GetObjectFromPrefabsAsync<T>();
        }

        private GameObject GetPoolObjectFromList<T>()
        {
            for (int i = PoolObjects.Count - 1; i >= 0; i--)
            {
                var poolObject = PoolObjects[i];
                var component = poolObject.GetComponent<T>();
                if (component != null && !poolObject.activeInHierarchy)
                {
                    poolObject.transform.SetParent(null);
                    poolObject.SetActive(true);
                    PoolObjects.RemoveAt(i);
                    return poolObject;
                }
            }

            return null;
        }

        private async UniTask<GameObject> GetObjectFromPrefabsAsync<T>()
        {
            await semaphore.WaitAsync();

            try
            {
                suitablePrefab = null;

                // Перебираем все префабы и ищем подходящий
                foreach (var prefabReference in poolPrefabReferences)
                {
                    // Загружаем префаб асинхронно
                    var prefabHandle = Addressables.LoadAssetAsync<GameObject>(prefabReference);
                    await prefabHandle.Task;

                    if (prefabHandle.Status ==
                        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        var prefab = prefabHandle.Result;
                        if (prefab != null && prefab.GetComponent<T>() != null)
                        {
                            suitablePrefab = prefabReference;
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to load prefab via Addressables.");
                    }

                    // Освобождаем ресурсы после завершения загрузки
                    Addressables.Release(prefabHandle);
                }

                // Если не нашли подходящий префаб, выбрасываем исключение
                if (suitablePrefab == null)
                {
                    throw new NullReferenceException("No suitable prefab found for the requested type.");
                }

                // Создаем объект из найденного префаба
                var handle = PhotonNetwork.Instantiate(suitablePrefab.AssetGUID, Vector3.zero, Quaternion.identity);
                photonView.RPC(nameof(InjectComponent), RpcTarget.All, handle.GetComponent<PhotonView>().ViewID);
                return handle;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void ReturnToPool(GameObject obj)
        {
            obj.transform.SetParent(poolParent);
            obj.SetActive(false);
            PoolObjects.Add(obj);
        }
    }
}