#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace AssetProcessing
{
    public static class SliceSprites
    {
        [MenuItem("Tools/Slice Sprites 2x1")]
        public static void Run2X1()
        {
            Run(2, 1);
        }

        [MenuItem("Tools/Slice Sprites 4x4")]
        public static void Run4X4()
        {
            Run(4, 4);
        }

        private static void Run(int nx, int ny)
        {
            AssetProcessor.IterateOverSelectedAssets(
                path =>
                {
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                    var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (textureImporter == null) return;

                    textureImporter.spriteImportMode = SpriteImportMode.Multiple;

                    var factory = new SpriteDataProviderFactories();
                    factory.Init();
                    var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
                    dataProvider.InitSpriteEditorDataProvider();

                    var spriteRects = new List<SpriteRect>();
                    for (var y = 0; y < ny; y++)
                    for (var x = 0; x < nx; x++)
                        spriteRects.Add(new SpriteRect
                        {
                            name = $"{texture.name}_{y}_{x}",
                            spriteID = GUID.Generate(),
                            rect = new Rect((float)x * texture.width / nx, (float)y * texture.height / ny,
                                (float)texture.width / nx,
                                (float)texture.height / ny)
                        });

                    dataProvider.SetSpriteRects(spriteRects.ToArray());

                    dataProvider.Apply();
                    EditorUtility.SetDirty(textureImporter);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            );
        }
    }
}
#endif