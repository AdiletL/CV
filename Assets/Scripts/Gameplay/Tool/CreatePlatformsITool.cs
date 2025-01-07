using System;
using Unit.Platform;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Tool
{
    [ExecuteInEditMode]
    public class CreatePlatformsITool : MonoBehaviour, IToolEditor
    {
        [SerializeField] private Vector2Int length;
        
        [Space]
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private bool isPaintCell;
        [SerializeField] private Transform parent;

        private CreateGameFieldITool createGameFieldITool;
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
        
        public void MarkDirty()
        {
            // Уведомляем Unity Editor о необходимости пересохранить объект
            EditorUtility.SetDirty(this);
        }

        private void Reset()
        {
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(1000, 1, 1000);
            plane.GetComponent<Renderer>().enabled = false;
        }

        private void CheckLink()
        {
            if(createGameFieldITool == null)
                createGameFieldITool = GetComponent<CreateGameFieldITool>();
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
            CheckClickedCell();
        }

        #region Cell
        private void CheckClickedCell()
        {
            Event e = Event.current; // Получение текущего события
            if (e == null) return;
            
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.CapsLock)
            {
                isPaintCell = !isPaintCell;
                Debug.LogWarning(isPaintCell ? "ENABLE: Paint Cell " : "DISABLE: Paint Cell");
            }
            
            if(!isPaintCell) return;

            // Проверка нажатий мыши
            if (e.type == EventType.MouseDown && e.button == 0) // Левая кнопка
            {
                PlacePrefabAtMousePosition();
            }
            else if (e.type == EventType.MouseDown && e.button == 2) // Правая кнопка
            {
                RemovePrefabAtMousePosition();
            }
        }
        
        private void PlacePrefabAtMousePosition()
        {
            CheckLink();
            if (!isPaintCell)
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
                if (Physics.CheckSphere(position, radius, Layers.PLATFORM_LAYER))
                {
                    Debug.LogWarning("На этой позиции уже есть объект");
                    return;
                }
                
                var newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab.gameObject);
                newGameObject.transform.position = position;
                newGameObject.transform.SetParent(parent);
            }

            MarkDirty();
            createGameFieldITool.CurrentGameField.SortingArray();
        }
        
        private void RemovePrefabAtMousePosition()
        {
            CheckLink();
            Vector3? mouseWorldPosition = GetMouseWorldPosition();
            if (mouseWorldPosition.HasValue)
            {
                Vector3 position = mouseWorldPosition.Value;

                // Поиск объекта на указанной позиции
                Collider[] colliders = Physics.OverlapSphere(position, radius, Layers.PLATFORM_LAYER);
                foreach (Collider col in colliders)
                {
                    if (col.gameObject == null) continue;
                    Undo.DestroyObjectImmediate(col.gameObject);
                    return;
                }
            }

            createGameFieldITool.CurrentGameField.SortingArray();
        }
        
        public void CreateCells()
        {
            CheckLink();
            if (isPaintCell)
            {
                Debug.LogError("Drawing platform included");
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
            createGameFieldITool.CurrentGameField.SortingArray();
        }

        public void DestroyCells()
        {
            CheckLink();
            if (parent == null)
            {
                Debug.LogError("Parent is null");
                return;
            }
            
            while (parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }

            MarkDirty();
            createGameFieldITool.CurrentGameField.SortingArray();
        }
        #endregion
    }
}