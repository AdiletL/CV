using UnityEditor;
using UnityEngine;

namespace Gameplay.Tool
{
        #if UNITY_EDITOR
    public abstract class ToolEditor : MonoBehaviour, IToolEditor
    {
        public void MarkDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
        #endif
}