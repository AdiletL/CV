﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unit.Character;
using Unit.Cell;
using Unit.Item;
using Unit.Trap;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay
{
    public class GameFieldController : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;
        [Inject] private GameUnits gameUnits;
        
        [SerializeField] private CellController[] platforms;
        [SerializeField] private CharacterMainController[] characters;
        [SerializeField] private TrapController[] traps;
        [FormerlySerializedAs("interactableObjects")] [SerializeField] private ItemController[] itemObjects;
        
        [field: SerializeField, Space(10)] public Transform StartPoint { get; private set; } 
        [field: SerializeField] public Transform EndPoint { get; private set; } 
        
        [field: SerializeField, Space(20)] public Transform PlayerSpawnPoint { get; private set; }
        
        private PhotonView photonView;
        
        private Stack<CellController> platformStack = new();
        private Stack<CharacterMainController> charactersStack = new();
        private Stack<TrapController> trapsStack = new();
        private Stack<ItemController> interactableObjectStack = new();
        
        
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
            platforms = GetComponentsInChildren<CellController>(true);
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
            
            photonView.RPC(nameof(InitializeCharacters), RpcTarget.AllBuffered);
            photonView.RPC(nameof(InitializeTraps), RpcTarget.AllBuffered);
            photonView.RPC(nameof(InitializeInteractableObjects), RpcTarget.AllBuffered);
            photonView.RPC(nameof(InitializeCells), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void InitializeCells()
        {
            foreach (var VARIABLE in platforms)
            {
                gameUnits.AddUnits(VARIABLE.gameObject);
                diContainer.Inject(VARIABLE);
                if (!VARIABLE.gameObject.activeSelf) continue;
                VARIABLE.Initialize();
                platformStack.Push(VARIABLE);
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
                if (!VARIABLE.gameObject.activeSelf) continue;
                VARIABLE.Initialize();
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
                if (!VARIABLE.gameObject.activeSelf) continue;
                VARIABLE.Initialize();
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
                if (!VARIABLE.gameObject.activeSelf) continue;
                VARIABLE.Initialize();
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
            foreach (var VARIABLE in platformStack)
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
            foreach (var VARIABLE in platformStack)
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
        
    }
}