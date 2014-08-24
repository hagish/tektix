using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Network))]
public class NetworkInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            Network t = (Network)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Piece Player 0"))
            {
                t.Player0.AddUnitToPool((Unit.UnitType)(int)(Random.Range(0, 3)), false);
            }
            if (GUILayout.Button("Add Piece Player 1"))
            {
                t.Player1.AddUnitToPool((Unit.UnitType)(int)(Random.Range(0, 3)), false);
            }
        }
    }
}