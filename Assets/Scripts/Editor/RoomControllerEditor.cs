using Gameplay;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomController))]
public class RoomControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var room = (RoomController)target;

        if (GUILayout.Button("SortingArray"))
        {
            room.SortingArray();
        }
    }
}
