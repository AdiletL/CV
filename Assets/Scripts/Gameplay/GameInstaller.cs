using System.Collections;
using Laboratory.Manager;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private AssetReference gameManagerPrefab;
    [SerializeField] private AssetReference laboratoryManagerPrefab;


    public override void InstallBindings()
    {
        Container = new DiContainer();
    }

    public override void Start()
    {
        base.Start();
        Initialize();
    }

    private void Initialize()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case Scenes.GAMEPLAY_NAME:
                var gameManagerObject = PhotonNetwork.Instantiate(gameManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
                var gameManager = gameManagerObject.GetComponent<Gameplay.Manager.GameManager>();
                Container.Inject(gameManager);
                gameManager.Initialize();
                //GameManagerInjectComponents();
                break;
            case Scenes.LABORATORY_NAME:
                var laboratoryManagerObject = PhotonNetwork.Instantiate(laboratoryManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
                var laboratoryManager = laboratoryManagerObject.GetComponent<LaboratoryManager>();
                Container.Inject(laboratoryManager);
                laboratoryManager.Initialize();
                //photonView.RPC(nameof(LaboratoryManagerInjectComponents), RpcTarget.AllBuffered, laboratoryManagerObject.GetComponent<PhotonView>().ViewID);
                break;
            default:
                
                break;
        }
    }

    [PunRPC]
    private void GameManagerInjectComponents(int viewID)
    {
        var gameManagerObject = PhotonView.Find(viewID).gameObject;
        var gameManager = gameManagerObject.GetComponent<Gameplay.Manager.GameManager>();
        Container.Inject(gameManager);
        gameManager.Initialize();
    }
    
    [PunRPC]
    private void LaboratoryManagerInjectComponents(int viewID)
    {
        var gameManagerObject = PhotonView.Find(viewID).gameObject;
        var gameManager = gameManagerObject.GetComponent<LaboratoryManager>();
        Container.Inject(gameManager);
        gameManager.Initialize();
    }
}
