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

    private PhotonView photonView;
    
    public string nextScene;

    public override void InstallBindings()
    {
        DontDestroyOnLoad(gameObject);
        Container = new DiContainer();
    }

    public override void Start()
    {
        base.Start();
        Initialize();
    }

    private void Initialize()
    {
        photonView = GetComponent<PhotonView>();
        if (!PhotonNetwork.IsMasterClient)  return;

        SceneManager.sceneLoaded += OnSceneLoaded;
        Scenes.TransitionToScene(nextScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Bootstrap") return;

        if (!PhotonNetwork.IsMasterClient)  return;
        
        switch (scene.name)
        {
            case Scenes.GAMEPLAY_NAME:
                var gameManagerObject = PhotonNetwork.Instantiate(gameManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
                photonView.RPC(nameof(GameManagerInjectComponents), RpcTarget.AllBuffered, gameManagerObject.GetComponent<PhotonView>().ViewID);
                break;
            case Scenes.LABORATORY_NAME:
                var laboratoryManagerObject = PhotonNetwork.Instantiate(laboratoryManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
                photonView.RPC(nameof(LaboratoryManagerInjectComponents), RpcTarget.AllBuffered, laboratoryManagerObject.GetComponent<PhotonView>().ViewID);
                break;
            default:
                Debug.LogError($"Unknown scene name: {scene.name}");
                break;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
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
