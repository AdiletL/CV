using Gameplay.Tool;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateBlockTool))]
public class CreateBlockToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CreateBlockTool cb = (CreateBlockTool)target;

        if (GUILayout.Button("Destroy Blocks"))
        {
            cb.DestroyBlocks();
        }
    }
}
