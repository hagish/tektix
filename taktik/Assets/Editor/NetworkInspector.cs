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

        }
    }
}