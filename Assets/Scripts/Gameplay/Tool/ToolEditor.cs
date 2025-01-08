using UnityEditor;
using UnityEngine;

namespace Gameplay.Tool
{
    public abstract class ToolEditor : MonoBehaviour, IToolEditor
    {
        public void MarkDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
}