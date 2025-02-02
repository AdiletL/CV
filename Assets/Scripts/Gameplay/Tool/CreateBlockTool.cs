using Unit.Cell;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Tool
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class CreateBlockTool : ToolEditor
    {
        [FormerlySerializedAs("blockPrefab")]
        [Space, Header("Prefab to create")]
        [SerializeField] private ObstacleGameObject obstaclePrefab;
        
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
            if (obstaclePrefab.GetComponent<ObstacleGameObject>() == null)
            {
                Debug.LogError("Prefab does not have a CellController");
                return;
            }
            if (radius == 0)
            {
                radius = .01f;
            }
            
            var hitObject = GetRayHitGameObject(e);
            if (hitObject)
            {
                var newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(obstaclePrefab.gameObject);
                newGameObject.transform.SetParent(hitObject.GetComponent<CellController>().VisualParent.transform);
                newGameObject.transform.localPosition = Vector3.zero;
            }

            MarkDirty();
            createGameFieldTool.CurrentRoom.SortingArray();
        }

        public void DestroyBlocks()
        {
            if (parent == null)
            {
                Debug.LogError("Parent is null");
                return;
            }
            
            var blocks = parent.GetComponentsInChildren<ObstacleGameObject>();

            for (int i = blocks.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(blocks[i].gameObject);
            }

            MarkDirty();
            createGameFieldTool.CurrentRoom.SortingArray();
        }
    }
        #endif
}