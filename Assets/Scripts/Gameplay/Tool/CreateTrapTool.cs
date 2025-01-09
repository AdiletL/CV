using Unit.Cell;
using Unit.Trap;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Tool
{
    [ExecuteInEditMode]
    public class CreateTrapTool : ToolEditor
    {
        [Space, Header("Prefab to create")]
        [SerializeField] private TrapController trapPrefab;
        
        [Space, Header("Place to destroy")]
        [SerializeField] private Transform parent;
        
        [Space, Header("Active/InActive (Button: Escape)"), Tooltip("Click button Escape")]
        [SerializeField] private bool isActive;
        
        private CreateGameFieldTool createGameFieldTool;
        private float radius;
        
        private GameObject GetRayHitGameObject(Event e)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, Layers.CELL_LAYER))
            {
                return hitInfo.transform.gameObject;
            }
            return null;
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
            
            // Проверка нажатий мыши
            if (e.type == EventType.MouseDown && e.button == 0) // Левая кнопка
            {
                PlacePrefabAtMousePosition(e);
            }
            else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape) 
            {
                isActive = !isActive;
                Debug.LogWarning(isActive ? "Active Block" : "InActive Block");
            }
        }
        
        private void PlacePrefabAtMousePosition(Event e)
        {
            if (!isActive)
            {
                return;
            }
            if (createGameFieldTool == null)
            {
                createGameFieldTool = GetComponent<CreateGameFieldTool>();
            }
            if (trapPrefab.GetComponent<TrapController>() == null)
            {
                Debug.LogError("Prefab does not have a CellController");
                return;
            }
            if (radius == 0)
            {
                radius = .01f;
                
            }
            
            var hitGameObject = GetRayHitGameObject(e);
            if(hitGameObject)
            {
                var newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(trapPrefab.gameObject);
                newGameObject.transform.SetParent(hitGameObject.GetComponent<CellController>().VisualParent.transform);
                newGameObject.transform.localPosition = Vector3.zero;
            }

            MarkDirty();
            createGameFieldTool.CurrentGameField.SortingArray();
        }

        public void DestroyTraps()
        {
            if (parent == null)
            {
                Debug.LogError("Parent is null");
                return;
            }
            
            var traps = parent.GetComponentsInChildren<TrapController>();

            for (int i = traps.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(traps[i].gameObject);
            }

            MarkDirty();
            createGameFieldTool.CurrentGameField.SortingArray();
        }
    }
}