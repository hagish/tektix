using UnityEngine;
using UnityEditor;
using System.Collections;

public static class UKEditorLayout {
    public class Horizonal : System.IDisposable
    {
        private bool disposed = false;

        public Horizonal()
        {
            EditorGUILayout.BeginHorizontal();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                EditorGUILayout.EndHorizontal();
                disposed = true;
            }
        }
    }

    public class Vertical : System.IDisposable
    {
        private bool disposed = false;

        public Vertical()
        {
            EditorGUILayout.BeginVertical();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                EditorGUILayout.EndVertical();
                disposed = true;
            }
        }
    }
}
