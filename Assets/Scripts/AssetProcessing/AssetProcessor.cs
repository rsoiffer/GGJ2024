using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetProcessing
{
    public static class AssetProcessor
    {
        internal static void IterateOverSelectedAssets(Action<string> action)
        {
            var paths = GetSelectedAssetPathsRecursive().ToList();
            try
            {
                AssetDatabase.StartAssetEditing(); // this isn't needed, but will make the process run a lot faster
                for (var i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];
                    EditorUtility.DisplayProgressBar("Processing assets", path, (float)i / paths.Count);
                    Debug.Log($"Processing {path}");
                    action(path);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
            }
        }

        internal static void IterateOverSelectedPrefabs(Func<GameObject, bool> action)
        {
            IterateOverSelectedAssets(
                path =>
                {
                    if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null) return;
                    if (Path.GetExtension(path) != ".prefab") return;
                    var loadedPrefab = PrefabUtility.LoadPrefabContents(path);
                    var edited = action(loadedPrefab);
                    if (edited) PrefabUtility.SaveAsPrefabAsset(loadedPrefab, path);
                    PrefabUtility.UnloadPrefabContents(loadedPrefab);
                }
            );
        }

        private static IEnumerable<string> GetSelectedAssetPathsRecursive()
        {
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path)) continue;

                if (Directory.Exists(path))
                    foreach (var guid in AssetDatabase.FindAssets("", new[] { path }))
                    {
                        var path2 = AssetDatabase.GUIDToAssetPath(guid);
                        if (!Directory.Exists(path2)) yield return path2;
                    }

                else if (File.Exists(path)) yield return path;
            }
        }
    }
}