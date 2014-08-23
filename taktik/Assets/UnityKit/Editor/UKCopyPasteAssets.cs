using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public static class UKCopyPasteAssets {
    public static List<string> CopiedAssets = new List<string>();

    [MenuItem("UnityKit/Assets/Cut Assets")]
    public static void Cut()
    {
        CopiedAssets.Clear();

        foreach (var a in Selection.objects)
        {
            if (a == null) return;
            var path = AssetDatabase.GetAssetPath(a);
            if (File.Exists(path))
            {
                Debug.Log(string.Format("cutted asset '{0}'", path));
                CopiedAssets.Add(path);
            }
        }
    }

    private static void TryToMove(string asset, string destination)
    {
        if (Directory.Exists(destination) && !string.IsNullOrEmpty(asset) && File.Exists(asset))
        {
            var assetFilename = Path.GetFileName(asset);
            var newPath = Path.Combine(destination, assetFilename);
            if (!File.Exists(newPath))
            {
                Debug.Log(string.Format("moving '{0}' to '{1}'", asset, newPath));
                AssetDatabase.MoveAsset(asset, newPath);
            }
            else
            {
                Debug.LogError(string.Format("there is already an asset '{0}'", newPath));
            }
        }
    }

    [MenuItem("UnityKit/Assets/Paste Asset To Folder")]
    public static void Paste()
    {
        Object a = Selection.activeObject;
        if (a == null) return;

        var path = AssetDatabase.GetAssetPath(a);

        foreach (var asset in CopiedAssets)
        {
            TryToMove(asset, path);
        }
    }
}
