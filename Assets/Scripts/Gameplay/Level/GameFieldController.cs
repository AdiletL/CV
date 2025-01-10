using System.Collections;
using System.Collections.Generic;
using Unit.Character;
using Unit.Cell;
using Unit.Trap;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class GameFieldController : MonoBehaviour
    {
        private DiContainer diContainer;
        private GameUnits gameUnits;
        
        [SerializeField] private CellController[] platforms;
        [SerializeField] private CharacterMainController[] characters;
        [SerializeField] private TrapController[] traps;
        
        [field: SerializeField, Space(10)] public Transform StartPoint { get; private set; } 
        [field: SerializeField] public Transform EndPoint { get; private set; } 
        
        [field: SerializeField, Space(20)] public Transform PlayerSpawnPoint { get; private set; }
        
        private Stack<CellController> platformStack = new();
        private Stack<CharacterMainController> charactersStack = new();
        private Stack<TrapController> trapsStack = new();
        
        [Inject]
        private void Construct(DiContainer diContainer, GameUnits gameUnits)
        {
            this.diContainer = diContainer;
            this.gameUnits = gameUnits;
        }
        
        
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
            InitializeCharacters();
            InitializeTraps();
            InitializeCells();
        }

        private void InitializeCells()
        {
            foreach (var VARIABLE in platforms)
            {
                gameUnits.AddUnits(VARIABLE);
                diContainer.Inject(VARIABLE);
                if (!VARIABLE.gameObject.activeInHierarchy) continue;
                VARIABLE.Initialize();
                platformStack.Push(VARIABLE);
                VARIABLE.Hide();
            }
        }
        private void InitializeCharacters()
        {
            foreach (var VARIABLE in characters)
            {
                gameUnits.AddUnits(VARIABLE);
                diContainer.Inject(VARIABLE);
                if (!VARIABLE.gameObject.activeInHierarchy) continue;
                VARIABLE.Initialize();
                charactersStack.Push(VARIABLE);
                VARIABLE.Hide();
            }
        }
        private void InitializeTraps()
        {
            foreach (var VARIABLE in traps)
            {
                gameUnits.AddUnits(VARIABLE);
                diContainer.Inject(VARIABLE);
                if (!VARIABLE.gameObject.activeInHierarchy) continue;
                VARIABLE.Initialize();
                trapsStack.Push(VARIABLE);
                VARIABLE.Hide();
            }
        }
        #endregion

        #region StartGame
        public void StartGame()
        {
            ShowPlatforms();
            ShowTraps();
            ShowCharacters();
            
            AppearPlatforms();
            AppearTraps();
            AppearCharacters();
        }

        #region Show
        private void ShowPlatforms()
        {
            foreach (var VARIABLE in platformStack)
                VARIABLE.Show();
        }
        private void ShowCharacters()
        {
            foreach (var VARIABLE in charactersStack)
                VARIABLE.Show();
        }
        private void ShowTraps()
        {
            foreach (var VARIABLE in trapsStack)
                VARIABLE.Show();
        }
        #endregion

        #region Appear
        private void AppearPlatforms()
        {
            foreach (var VARIABLE in platformStack)
                VARIABLE.Appear();
        }
        private void AppearCharacters()
        {
            foreach (var VARIABLE in charactersStack)
                VARIABLE.Appear();
        }
        private void AppearTraps()
        {
            foreach (var VARIABLE in trapsStack)
                VARIABLE.Appear();
        }
        #endregion
        #endregion
        
    }
}