using Unit.Cell;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
    public class DefaultCell : MonoBehaviour
    {
        public Vector2Int Coordinates { get; private set; }
        
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
            }

            MarkDirty();
        }
            
        private void MarkDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
}