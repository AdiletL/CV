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
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay
{
    public class RoomController : MonoBehaviour, IActivatable
    {
        [Inject] private DiContainer diContainer;
        [Inject] private GameUnits gameUnits;

        public static Action<int> OnTriggerSpawnNextRoom;
        
        [SerializeField] private NextRoomConfigHandler[] nextRoomConfigHandler;
        
        [Space(10)]
        [SerializeField] private CellController[] cells;
        [SerializeField] private CharacterMainController[] characters;
        [SerializeField] private TrapController[] traps;
        [SerializeField] private ContainerController[] containers;
        
        [field: SerializeField, Space(10)] public NavMeshControl NavMeshControl { get; private set; }
        [field: SerializeField, Space(20)] public Transform PlayerSpawnPoint { get; private set; }
        
        private PhotonView photonView;
        private int countCreep;
        
        public int ID { get; private set; }
        public bool IsActive { get; protected set; }
        
        
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
            nextRoomConfigHandler = GetComponentsInChildren<NextRoomConfigHandler>(true);

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
            photonView.RPC(nameof(InitializeContainers), RpcTarget.AllBuffered);

            SubscribeEvent();
        }

        [PunRPC]
        private void InitializeCells()
        {
            foreach (var VARIABLE in cells)
            {
                gameUnits.AddUnits(VARIABLE.gameObject);
                diContainer.Inject(VARIABLE);
                VARIABLE.Initialize();
                VARIABLE.Deactivate();
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
                VARIABLE.Deactivate();
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
                VARIABLE.Deactivate();
            }
        }
        [PunRPC]
        private void InitializeContainers()
        {
            foreach (var VARIABLE in containers)
            {
                gameUnits.AddUnits(VARIABLE.gameObject);
                diContainer.Inject(VARIABLE);
                VARIABLE.Initialize();
                VARIABLE.Deactivate();
            }
        }
        #endregion

        private void SubscribeEvent()
        {
            foreach (var VARIABLE in characters)
            {
                if (VARIABLE is CreepController creepController)
                {
                    creepController.OnDeath += OnCreepDeath;
                    countCreep++;
                }
            }

            foreach (var VARIABLE in nextRoomConfigHandler)
                VARIABLE.OnTrigger += OnTriggerNextRoom;
        }
        private void UnSubscribeEvent()
        {
            foreach (var VARIABLE in characters)
            {
                if (VARIABLE is CreepController creepController)
                    creepController.OnDeath -= OnCreepDeath;
            }
            
            foreach (var VARIABLE in nextRoomConfigHandler)
                VARIABLE.OnTrigger -= OnTriggerNextRoom;
        }

        #region StartGame
        public void Activate()
        {
            photonView.RPC(nameof(ActivateCells), RpcTarget.AllBuffered);
            photonView.RPC(nameof(ActivateTraps), RpcTarget.AllBuffered);
            photonView.RPC(nameof(ActivateCharacters), RpcTarget.AllBuffered);
            photonView.RPC(nameof(ActivateContainers), RpcTarget.AllBuffered);
            IsActive = true;
        }

        public void Deactivate()
        {
            photonView.RPC(nameof(DeactivateCells), RpcTarget.AllBuffered);
            photonView.RPC(nameof(DeactivateTraps), RpcTarget.AllBuffered);
            photonView.RPC(nameof(DeactivateCharacters), RpcTarget.AllBuffered);
            photonView.RPC(nameof(DeactivateContainers), RpcTarget.AllBuffered);
            IsActive = false;
        }
        
        #region Activate
        [PunRPC]
        private void ActivateCells()
        {
            foreach (var VARIABLE in cells)
                if(VARIABLE.gameObject.activeSelf) VARIABLE.Activate();
        }
        [PunRPC]
        private void ActivateCharacters()
        {
            foreach (var VARIABLE in characters)
                if(VARIABLE.gameObject.activeSelf) VARIABLE.Activate();
        }
        [PunRPC]
        private void ActivateTraps()
        {
            foreach (var VARIABLE in traps)
                if(VARIABLE.gameObject.activeSelf) VARIABLE.Activate();
        }
        [PunRPC]
        private void ActivateContainers()
        {
            foreach (var VARIABLE in containers)
                if(VARIABLE.gameObject.activeSelf) VARIABLE.Activate();
        }
        #endregion
        
        #region Deactivate
        [PunRPC]
        private void DeactivateCells()
        {
            foreach (var VARIABLE in cells)
                VARIABLE.Deactivate();
        }
        [PunRPC]
        private void DeactivateCharacters()
        {
            foreach (var VARIABLE in characters)
                VARIABLE.Deactivate();
        }
        [PunRPC]
        private void DeactivateTraps()
        {
            foreach (var VARIABLE in traps)
                VARIABLE.Deactivate();
        }
        [PunRPC]
        private void DeactivateContainers()
        {
            foreach (var VARIABLE in containers)
                VARIABLE.Deactivate();
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
            UnSubscribeEvent();
        }
    }
}