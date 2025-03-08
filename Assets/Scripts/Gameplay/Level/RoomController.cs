using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.NavMesh;
using Gameplay.Unit.Cell;
using Gameplay.Unit.Character;
using Gameplay.Unit.Character.Creep;
using Gameplay.Unit.Container;
using Gameplay.Unit.Trap;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class RoomController : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;
        [Inject] private GameUnits gameUnits;

        public static Action<int> OnTriggerSpawnNextRoom;
        
        [SerializeField] private NextRoomContainer[] nextRoomContainers;
        
        [Space(10)]
        [SerializeField] private CellController[] cells;
        [SerializeField] private CharacterMainController[] characters;
        [SerializeField] private TrapController[] traps;
        [SerializeField] private ContainerController[] containers;
        
        [field: SerializeField, Space(10)] public NavMeshControl NavMeshControl { get; private set; }
        [field: SerializeField, Space(20)] public Transform PlayerSpawnPoint { get; private set; }
        
        private PhotonView photonView;
        private int countCreep;
        
        private Stack<CellController> cellStack = new();
        private Stack<CharacterMainController> charactersStack = new();
        private Stack<TrapController> trapsStack = new();
        private Stack<ContainerController> interactableObjectStack = new();
        
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
            containers = GetComponentsInChildren<ContainerController>(true);
            nextRoomContainers = GetComponentsInChildren<NextRoomContainer>(true);

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

            InitializeMediator();
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
            foreach (var VARIABLE in containers)
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

        private void InitializeMediator()
        {
            foreach (var VARIABLE in characters)
            {
                if (VARIABLE is CreepController creepController)
                {
                    creepController.OnDeath += OnCreepDeath;
                    countCreep++;
                }
            }

            foreach (var VARIABLE in nextRoomContainers)
                VARIABLE.OnTrigger += OnTriggerNextRoom;
        }
        private void DeInitializeMediator()
        {
            foreach (var VARIABLE in characters)
            {
                if (VARIABLE is CreepController creepController)
                    creepController.OnDeath -= OnCreepDeath;
            }
            
            foreach (var VARIABLE in nextRoomContainers)
                VARIABLE.OnTrigger -= OnTriggerNextRoom;
        }

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

        private void OnTriggerNextRoom(int id)
        {
            SpawnNextRoom(id);
        }

        private void SpawnNextRoom(int id)
        {
            OnTriggerSpawnNextRoom?.Invoke(id);
        }

        private void OnCreepDeath(CreepController creepController)
        {
            countCreep--;
            if (countCreep == 0)
            {
                Debug.Log($"Room {ID}: {characters.Length}/{countCreep}");
            }
        }
        

        private void OnDestroy()
        {
            DeInitializeMediator();
        }
    }
}