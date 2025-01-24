using Unit.Cell;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
    #if UNITY_EDITOR
    public class DefaultCell : MonoBehaviour
    {
        [field: SerializeField] public Vector2Int Coordinates { get; private set; }
        
        public void SetCoordinates(int x, int y)
        {
            this.Coordinates = new Vector2Int(x, y);
            MarkDirty();
        }

        public void SettingGameCell()
        {
            var colliders = Physics.OverlapSphere(transform.position, .2f, Layers.CELL_LAYER);
            if (colliders.Length > 0)
            {
                colliders[0].transform.GetComponent<CellController>().SetCoordinates(Coordinates);
                colliders[0].transform.position = new Vector3(transform.position.x, colliders[0].transform.position.y, transform.position.z);
            }

            MarkDirty();
        }
            
        private void MarkDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
    #endif
}