using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorSceneSwitcher : MonoBehaviour {
    [MenuItem("Scenes/bootstrap")]
    static void Open0()
    {
        EditorApplication.OpenScene("Assets/Scenes/bootstrap.unity");
    }

    [MenuItem("Scenes/game")]
    static void Open1()
    {
        EditorApplication.OpenScene("Assets/Scenes/game.unity");
    }

    [MenuItem("Scenes/test")]
    static void Open2()
    {
        EditorApplication.OpenScene("Assets/Scenes/test.unity");
    }

}
