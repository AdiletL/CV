using Unit.Cell;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Tool
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CreateDefaultCellTool))]
    public class CreateCellTool : ToolEditor
    {
        [Space, Header("Prefab to create")]
        [SerializeField] private GameObject cellPrefab;
        
        [Space, Header("Place to spawn")]
        [SerializeField] private Transform parent;
        
        [Space, Header("Active/InActive (Button: Escape)"), Tooltip("Click button Escape")]
        [SerializeField] private bool isActive;
        
        private CreateGameFieldTool createGameFieldTool;
        private float radius;
        
        private GameObject GetRayHitGameObject(Event e)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity)
                && hitInfo.transform.TryGetComponent(out DefaultCell defaultCell))
            {
                return hitInfo.transform.gameObject;
            }
            return null;
        }
        
        private void CheckLink()
        {
            if(createGameFieldTool == null)
                createGameFieldTool = GetComponent<CreateGameFieldTool>();
            if (radius == 0)
                radius = cellPrefab.transform.localScale.x / 2.5f;
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
                Debug.LogWarning(isActive ? "Active Paint Cell " : "InActive Paint Cell");
            }
        }
        
        private void PlacePrefabAtMousePosition(Event e)
        {
            CheckLink();
            if (!isActive)
            {
                return;
            }
            else if (cellPrefab.GetComponent<CellController>() == null)
            {
                Debug.LogError("Prefab does not have a CellController");
                return;
            }
            else if (parent == null)
            {
                Debug.LogError("Parent is null");
                return;
            }
            
            var hitObject = GetRayHitGameObject(e);
            if (hitObject)
            {
                var newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab.gameObject);
                newGameObject.transform.position = hitObject.transform.position;
                newGameObject.transform.SetParent(parent);
                
                var cellController = newGameObject.GetComponent<CellController>();
                var defaultCell = hitObject.GetComponent<DefaultCell>();
                cellController.SetCoordinates(defaultCell.Coordinates);
            }

            MarkDirty();
            createGameFieldTool.CurrentGameField.SortingArray();
        }
        

        public void DestroyCells()
        {
            CheckLink();
            if (parent == null)
            {
                Debug.LogError("Parent is null");
                return;
            }
            
            var cells = parent.GetComponentsInChildren<CellController>();

            for (int i = cells.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(cells[i].gameObject);
            }

            MarkDirty();
            createGameFieldTool.CurrentGameField.SortingArray();
        }
    }
}