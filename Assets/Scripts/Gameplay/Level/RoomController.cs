using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.NavMesh;
using Photon.Pun;
using Unit.Character;
using Unit.Cell;
using Unit.Character.Player;
using Unit.Item;
using Unit.Trap;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay
{
    public class RoomController : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;
        [Inject] private GameUnits gameUnits;

        public static Action<int, List<Vector3>> OnSpawnNextRooms;
        
        [FormerlySerializedAs("platforms")]
        [Space(10)]
        [SerializeField] private CellController[] cells;
        [SerializeField] private CharacterMainController[] characters;
        [SerializeField] private TrapController[] traps;
        [SerializeField] private ItemController[] itemObjects;
        
        [field: SerializeField, Space(10)] public NavMeshControl NavMeshControl { get; private set; }
        [field: SerializeField, Space(10)] public Transform StartPoint { get; private set; } 
        [field: SerializeField] public Transform[] EndPoints { get; private set; } 
        [field: SerializeField, Space(20)] public Transform PlayerSpawnPoint { get; private set; }
        
        private PhotonView photonView;
        private bool isTriggerPlayer;
        
        private Stack<CellController> cellStack = new();
        private Stack<CharacterMainController> charactersStack = new();
        private Stack<TrapController> trapsStack = new();
        private Stack<ItemController> interactableObjectStack = new();
        
        public int ID { get; private set; }
        
        
        #region Editor
        #if UNITY_EDITOR
        private Coroutine editorCoroutine;
        private int currentSort = 0;
        
        public void SortingArray()
        {
            if(editorCoroutine != null) StopCoroutine(editorCoroutine);
            editorCoroutine = StartCoroutine(StartAllSortingCoroutine());
        }

        private IEnumerator StartAllSortingCoroutine()
        {
            cells = GetComponentsInChildren<CellController>(true);
            characters = GetComponentsInChildren<CharacterMainController>(true);
            traps = GetComponentsInChildren<TrapController>(true);
            itemObjects = GetComponentsInChildren<ItemController>(true);

            yield return null;
            MarkDirty();
            //Debug.Log("Finished Sorting");
        }
        
        public void MarkDirty()
        {
            // Уведомляем Unity Editor о необходимости пересохранить объект
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
        #endregion

        #region Initialize
        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
            
            photonView.RPC(nameof(InitializeCells), RpcTarget.AllBuffered);
            photonView.RPC(nameof(InitializeCharacters), RpcTarget.AllBuffered);
            photonView.RPC(nameof(InitializeTraps), RpcTarget.AllBuffered);
            photonView.RPC(nameof(InitializeInteractableObjects), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void InitializeCells()
        {
            foreach (var VARIABLE in cells)
            {
                gameUnits.AddUnits(VARIABLE.gameObject);
                diContainer.Inject(VARIABLE);
                VARIABLE.Initialize();
                if (!VARIABLE.gameObject.activeSelf) continue;
                cellStack.Push(VARIABLE);
                VARIABLE.Hide();
            }
        }
        [PunRPC]
        private void InitializeCharacters()
        {
            foreach (var VARIABLE in characters)
            {
                gameUnits.AddUnits(VARIABLE.gameObject);
                diContainer.Inject(VARIABLE);
                VARIABLE.Initialize();
                if (!VARIABLE.gameObject.activeSelf) continue;
                charactersStack.Push(VARIABLE);
                VARIABLE.Hide();
            }
        }
        [PunRPC]
        private void InitializeTraps()
        {
            foreach (var VARIABLE in traps)
            {
                gameUnits.AddUnits(VARIABLE.gameObject);
                diContainer.Inject(VARIABLE);
                VARIABLE.Initialize();
                if (!VARIABLE.gameObject.activeSelf) continue;
                trapsStack.Push(VARIABLE);
                VARIABLE.Hide();
            }
        }
        [PunRPC]
        private void InitializeInteractableObjects()
        {
            foreach (var VARIABLE in itemObjects)
            {
                gameUnits.AddUnits(VARIABLE.gameObject);
                diContainer.Inject(VARIABLE);
                VARIABLE.Initialize();
                if (!VARIABLE.gameObject.activeSelf) continue;
                interactableObjectStack.Push(VARIABLE);
                VARIABLE.Hide();
            }
        }
        #endregion

        #region StartGame
        public void StartGame()
        {
            photonView.RPC(nameof(ShowCells), RpcTarget.AllBuffered);
            photonView.RPC(nameof(ShowTraps), RpcTarget.AllBuffered);
            photonView.RPC(nameof(ShowCharacters), RpcTarget.AllBuffered);
            photonView.RPC(nameof(ShowInteractableObjects), RpcTarget.AllBuffered);
            
            photonView.RPC(nameof(AppearCells), RpcTarget.AllBuffered);
            photonView.RPC(nameof(AppearTraps), RpcTarget.AllBuffered);
            photonView.RPC(nameof(AppearCharacters), RpcTarget.AllBuffered);
            photonView.RPC(nameof(AppearInteractableObjects), RpcTarget.AllBuffered);
            //ShowCells();
            //ShowTraps();
            //ShowCharacters();
            //ShowInteractableObjects();
            
            //AppearCells();
            //AppearTraps();
            //AppearCharacters();
           // AppearInteractableObjects();
        }

        #region Show
        [PunRPC]
        private void ShowCells()
        {
            foreach (var VARIABLE in cellStack)
                VARIABLE.Show();
        }
        [PunRPC]
        private void ShowCharacters()
        {
            foreach (var VARIABLE in charactersStack)
                VARIABLE.Show();
        }
        [PunRPC]
        private void ShowTraps()
        {
            foreach (var VARIABLE in trapsStack)
                VARIABLE.Show();
        }
        [PunRPC]
        private void ShowInteractableObjects()
        {
            foreach (var VARIABLE in interactableObjectStack)
                VARIABLE.Show();
        }
        #endregion

        #region Appear
        [PunRPC]
        private void AppearCells()
        {
            foreach (var VARIABLE in cellStack)
                VARIABLE.Appear();
        }
        [PunRPC]
        private void AppearCharacters()
        {
            foreach (var VARIABLE in charactersStack)
                VARIABLE.Appear();
        }
        [PunRPC]
        private void AppearTraps()
        {
            foreach (var VARIABLE in trapsStack)
                VARIABLE.Appear();
        }
        [PunRPC]
        private void AppearInteractableObjects()
        {
            foreach (var VARIABLE in interactableObjectStack)
                VARIABLE.Appear();
        }
        #endregion
        #endregion
        
        public void SetID(int id) => this.ID = id;

        private void SpawnNextRooms()
        {
            var endPositions = new List<Vector3>(EndPoints.Length);
            for (int i = 0; i < EndPoints.Length; i++)
                endPositions.Add(EndPoints[i].position);
            
            OnSpawnNextRooms?.Invoke(ID, endPositions);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(isTriggerPlayer) return;
            if (other.TryGetComponent(out PlayerController player))
            {
                isTriggerPlayer = true;
                SpawnNextRooms();
            }
        }
    }
}