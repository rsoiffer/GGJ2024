#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetProcessing
{
    public static class DeleteUnusedAssets
    {
        [MenuItem("Tools/Delete Unused Assets")]
        public static void Run()
        {
            var agree = EditorUtility.DisplayDialog(
                "Delete unused assets?",
                "You cannot undo this action. Make sure you've committed your work before proceeding.",
                "Delete",
                "Cancel"
            );
            if (!agree) return;

            var allScenes = EditorBuildSettings.scenes.Select(scene => scene.path);

            var allDependencies = allScenes
                .SelectMany(path => AssetDatabase.GetDependencies(path, true))
                .ToHashSet();
            foreach (var s in allDependencies) Debug.Log(s);

            AssetProcessor.IterateOverSelectedAssets(
                asset =>
                {
                    if (allDependencies.Contains(asset)) return;
                    Debug.LogWarning($"Deleting {asset}");
                    AssetDatabase.DeleteAsset(asset);
                }
            );
        }
    }
}
#endif