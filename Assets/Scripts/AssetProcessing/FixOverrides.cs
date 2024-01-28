using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetProcessing
{
    public static class FixOverrides
    {
        [MenuItem("Tools/Fix Overrides")]
        public static void Run()
        {
            AssetProcessor.IterateOverSelectedPrefabs(
                loadedPrefab =>
                {
                    var edited = false;

                    var childObjects = loadedPrefab.GetComponentsInChildren<Transform>(true)
                        .Select(transform => transform.gameObject as Object)
                        .Concat(loadedPrefab.GetComponentsInChildren<Component>(true));
                    foreach (var child in childObjects)
                    {
                        var thisObject = new SerializedObject(child);
                        var thisProp = thisObject.GetIterator();
                        while (thisProp.Next(true))
                        {
                            if (!thisProp.prefabOverride) continue;
                            if (thisProp.isDefaultOverride) continue;
                            var source = PrefabUtility.GetCorrespondingObjectFromSource(child);
                            var sourceObject = new SerializedObject(source);
                            var sourceProp = sourceObject.FindProperty(thisProp.propertyPath);
                            if (!SerializedProperty.DataEquals(thisProp, sourceProp)) continue;
                            PrefabUtility.RevertPropertyOverride(thisProp, InteractionMode.AutomatedAction);
                            edited = true;
                        }
                    }

                    return edited;
                }
            );
        }
    }
}