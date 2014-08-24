using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Network))]
public class NetworkInspector : Editor
{
    private string text = "";

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
            text = EditorGUILayout.TextField("Out:", text);
            if (GUILayout.Button("Send To Player 0"))
            {
                t.ServerPlayer0.SendOutgoingMessage(text);
            }
            if (GUILayout.Button("Send To Player 1"))
            {
                t.ServerPlayer1.SendOutgoingMessage(text);
            }
        }
    }
}