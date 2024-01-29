#if UNITY_EDITOR
using System;
using UnityEditor;

namespace AssetProcessing
{
    public static class ForceReserialize
    {
        [MenuItem("Tools/Force Reserialize")]
        public static void Run()
        {
            AssetProcessor.IterateOverSelectedAssets(
                asset => { AssetDatabase.ForceReserializeAssets(new[] { asset }); }
            );
        }

        [MenuItem("Tools/Force Reserialize Materials")]
        public static void RunMaterialsOnly()
        {
            AssetProcessor.IterateOverSelectedAssets(
                asset =>
                {
                    if (asset.EndsWith(".mat", StringComparison.Ordinal))
                        AssetDatabase.ForceReserializeAssets(new[] { asset });
                }
            );
        }

        [MenuItem("Tools/Force Reserialize Non Vendor")]
        public static void ReserializeNonVendor()
        {
            foreach (var folder in AssetDatabase.GetSubFolders("Assets"))
            {
                if (folder is null or "Assets/Scenes" or "Assets/Vendor") continue;

                foreach (var guid in AssetDatabase.FindAssets("", new[] { folder }))
                {
                    var path2 = AssetDatabase.GUIDToAssetPath(guid);
                    AssetDatabase.ForceReserializeAssets(new[] { path2 });
                }
            }
        }
    }
}
#endif