#if UNITY_EDITOR
using UnityEditor;

namespace AssetProcessing
{
    public static class SetPixelsPerUnit
    {
        [MenuItem("Tools/Set Pixels Per Unit")]
        public static void Run()
        {
            AssetProcessor.IterateOverSelectedAssets(
                path =>
                {
                    var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (textureImporter == null) return;

                    textureImporter.spritePixelsPerUnit = 64;

                    EditorUtility.SetDirty(textureImporter);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            );
        }
    }
}
#endif