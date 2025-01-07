using System;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Tool
{
    [RequireComponent(typeof(CreatePlatformsITool))]
    public class CreateGameFieldITool : MonoBehaviour, IToolEditor
    {
        [SerializeField] private GameFieldController gameFieldPrefab;

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

        public void MarkDirty()
        {
            // Уведомляем Unity Editor о необходимости пересохранить объект
            EditorUtility.SetDirty(this);
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
            if (CurrentGameField == null)
            {
                CurrentGameField = GetComponentInChildren<GameFieldController>();
            }
            if (CurrentGameField == null)
            {
                Debug.LogError("Link on current game field is null");
                return;
            }
            
            CurrentGameField.SortingArray();
        }

        public void CreateGameField()
        {
            var gameField = (GameObject)PrefabUtility.InstantiatePrefab(gameFieldPrefab.gameObject, transform);
            gameField.transform.localPosition = Vector3.zero;
            CurrentGameField = gameField.GetComponent<GameFieldController>();
            MarkDirty();
        }
    }
}