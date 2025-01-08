using System;
using Unit.Cell;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Tool
{
    [ExecuteInEditMode]
    public class CreateCellTool : ToolEditor
    {
        [SerializeField] private Vector2Int length;
        
        [Space, Header("Prefab to create")]
        [SerializeField] private GameObject cellPrefab;
        
        [Space, Header("Place to spawn")]
        [SerializeField] private Transform parent;
        
        [Space, Header("Active/InActive (Button: Escape)"), Tooltip("Click button Escape")]
        [SerializeField] private bool isActive;

        private CreateGameFieldTool createGameFieldTool;
        private float radius;
        
        private Vector3? GetMouseWorldPosition()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                return hitInfo.point;
            }
            return null;
        }

        private void Reset()
        {
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(1000, 1, 1000);
            plane.GetComponent<Renderer>().enabled = false;
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
                PlacePrefabAtMousePosition();
            }
            else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                isActive = !isActive;
                Debug.LogWarning(isActive ? "Active Paint Cell " : "InActive Paint Cell");
            }
            /*else if (e.type == EventType.MouseDown && e.button == 2) // Правая кнопка
            {
                RemovePrefabAtMousePosition();
            }*/
        }
        
        private void PlacePrefabAtMousePosition()
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
            
            Vector3? mouseWorldPosition = GetMouseWorldPosition();
            if (mouseWorldPosition.HasValue)
            {
                Vector3 position = mouseWorldPosition.Value;
                position.y = 0;
                // Проверяем, есть ли объект с нужным скриптом
                if (Physics.CheckSphere(position, radius, Layers.CELL_LAYER))
                {
                    Debug.LogWarning("На этой позиции уже есть объект");
                    return;
                }
                
                var newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab.gameObject);
                newGameObject.transform.position = position;
                newGameObject.transform.SetParent(parent);
            }

            MarkDirty();
            createGameFieldTool.CurrentGameField.SortingArray();
        }
        
        private void RemovePrefabAtMousePosition()
        {
            CheckLink();
            Vector3? mouseWorldPosition = GetMouseWorldPosition();
            if (mouseWorldPosition.HasValue)
            {
                Vector3 position = mouseWorldPosition.Value;

                // Поиск объекта на указанной позиции
                Collider[] colliders = Physics.OverlapSphere(position, radius, Layers.CELL_LAYER);
                foreach (Collider col in colliders)
                {
                    if (col.gameObject == null) continue;
                    Undo.DestroyObjectImmediate(col.gameObject);
                    return;
                }
            }

            createGameFieldTool.CurrentGameField.SortingArray();
        }
        
        public void CreateCells()
        {
            CheckLink();
            if (cellPrefab.GetComponent<CellController>() == null)
            {
                Debug.LogError("Prefab does not have a CellController");
                return;
            }
            else if (parent == null)
            {
                Debug.LogError("Parent is null");
                return;
            }
            
            float intervalX = 0;
            float intervalZ = 0;
            for (var x = 0; x < length.x; x++)
            {
                intervalX = x * .05f;
                for (var y = 0; y < length.y; y++)
                {
                    intervalZ = y * .05f;
                    
                    var newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab.gameObject, parent.transform);
                    newGameObject.transform.localPosition = new Vector3(
                        (x * cellPrefab.transform.localScale.x) + intervalX, 0,
                        (y * cellPrefab.transform.localScale.z) + intervalZ);
                }
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