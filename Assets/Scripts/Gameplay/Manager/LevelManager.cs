using Cysharp.Threading.Tasks;
using Photon.Pun;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Manager
{
    public class LevelManager : MonoBehaviourPun, IManager
    {
        [Inject] private DiContainer diContainer;
        [Inject] private GameUnits gameUnits;
        [Inject] private RoomManager roomManager;

        [SerializeField] private AssetReferenceGameObject playerPrefab;
        [SerializeField] private GameObject cameraPrefab;

        private PlayerController playerController;
        
        private PhotonView photonView;

        public static int CurrentLevelIndex { get; private set; }
        

        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
        }
        
        public void StartLevel()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(CreateCamera), RpcTarget.AllBuffered);
                roomManager.SpawnStartRoom(CurrentLevelIndex);
            }

            CreatePlayer();
        }

        public void RestartLevel()
        {
            // Реализация перезапуска уровня
        }

        public void StopLevel()
        {
            // Реализация остановки уровня
        }

        [PunRPC]
        private void CreateCamera()
        {
            Instantiate(cameraPrefab);
        }

        private void CreatePlayer()
        {
            var playerGameObject = PhotonNetwork.Instantiate(playerPrefab.AssetGUID,
                roomManager.PlayerSpawnPosition, Quaternion.identity);
            
            photonView.RPC(nameof(InitializePlayer), RpcTarget.AllBuffered, playerGameObject.GetComponent<PhotonView>().ViewID);
        }
        
        [PunRPC]
        private void InitializePlayer(int viewID)
        {
            var playerGameObject = PhotonView.Find(viewID).gameObject;
            playerController = playerGameObject.GetComponent<PlayerController>();
            diContainer.Inject(playerController);
            diContainer.Bind<PlayerController>().FromInstance(playerController);
            
            var characterController = playerGameObject.GetComponent<CharacterController>();
            characterController.enabled = false;
            playerController.Initialize();
            characterController.enabled = true;

            gameUnits.AddUnits(playerController.gameObject);
            playerController.Appear();
        }
    }
}
