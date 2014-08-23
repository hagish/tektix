using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AudioDb))]
public class AudioDbInspector : Editor
{

    public override void OnInspectorGUI() 
    {
        AudioDb t = (AudioDb)target;

        if (t.Sounds != null)
        {
            foreach (var x in t.Sounds)
            { 
            
            }
        }

        DrawDefaultInspector();
    }
}
