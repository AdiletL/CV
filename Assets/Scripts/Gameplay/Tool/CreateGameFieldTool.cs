using System;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Tool
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    [RequireComponent(typeof(CreateCellTool))]
    [RequireComponent(typeof(CreateBlockTool))]
    [RequireComponent(typeof(CreateTrapTool))]
    public class CreateGameFieldTool : ToolEditor
    {
        [SerializeField] private GameObject gameFieldPrefab;

        private CreateCellTool createCellTool;
        private CreateBlockTool createBlockTool;
        private CreateTrapTool createTrapTool;

        private int maxIndex;
        private int countIndex;
        
        private GameFieldController gameFieldController;
        public GameFieldController CurrentGameField
        {
            get
            {
                if(!gameFieldController)
                    gameFieldController = GetComponentInChildren<GameFieldController>();
                
                return gameFieldController;
            }
            set => gameFieldController = value;
        }

        private void CheckLink()
        {
            if (CurrentGameField == null)
            {
                CurrentGameField = GetComponentInChildren<GameFieldController>();
            }
            if (CurrentGameField == null)
            {
                Debug.LogError("Link on current game field is null");
                return;
            }
            if (createBlockTool == null)
            {
                createBlockTool = GetComponent<CreateBlockTool>();
                maxIndex++;
            }
            if (createCellTool == null)
            {
                createCellTool = GetComponent<CreateCellTool>();
                maxIndex++;
            }
            if (createTrapTool == null)
            {
                createTrapTool = GetComponent<CreateTrapTool>();
                maxIndex++;
            }
        }
        

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI; // Подписываемся на события
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI; // Отписываемся от событий
        }
        
        private void OnSceneGUI(SceneView sceneView)
        {
            CheckClicked();
        }

        private void CheckClicked()
        {
            Event e = Event.current; // Получение текущего события
            if (e == null) return;
            
            if (e.type == EventType.KeyDown && e.shift)
            {
                CheckLink();
                Selection.activeGameObject = this.gameObject;
            }
            else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.CapsLock)
            {
                CheckLink();
                if(countIndex == maxIndex) countIndex = 1;
                else countIndex++;

                createCellTool.enabled = false;
                createBlockTool.enabled = false;
                createTrapTool.enabled = false;
                
                switch (countIndex)
                {
                    case 1:
                        createCellTool.enabled = true;
                        Debug.LogWarning("Switch on Cell");
                        break;
                    case 2:
                        createBlockTool.enabled = true;
                        Debug.LogWarning("Switch on Block");
                        break;
                    case 3:
                        createTrapTool.enabled = true;
                        Debug.LogWarning("Switch on Trap");
                        break;
                    
                    default:
                        break;
                }
            }
        }

        public void CreateGameField()
        {
            var gameField = (GameObject)PrefabUtility.InstantiatePrefab(gameFieldPrefab, transform);
            gameField.transform.localPosition = Vector3.zero;
            CurrentGameField = gameField.GetComponent<GameFieldController>();
            MarkDirty();
        }
    }
        #endif
}