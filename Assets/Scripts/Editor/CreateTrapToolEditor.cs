using Gameplay.Tool;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateTrapTool))]
public class CreateTrapToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CreateTrapTool trapTool = (CreateTrapTool)target;

        if (GUILayout.Button("Destroy Traps"))
        {
            trapTool.DestroyTraps();
        }
    }
}
