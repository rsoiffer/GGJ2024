using System;
using System.Linq;
using AssetProcessing;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "MoveDatabase", menuName = "Move Database", order = 0)]
    public class MoveDatabase : ScriptableObject
    {
        public MoveData[] database;

        [MenuItem("Tools/Build MoveDatabase")]
        public static void BuildDatabase()
        {
            AssetProcessor.IterateOverSelectedAssets(path =>
            {
                var moveDatabase = AssetDatabase.LoadAssetAtPath<MoveDatabase>(path);
                if (moveDatabase == null) return;
                var pbsDatabase = PbsDatabase.LoadDatabase("moves");
                moveDatabase.database =
                    pbsDatabase.Entries.Values.Select(pbsEntry => new MoveData(pbsEntry)).ToArray();
                EditorUtility.SetDirty(moveDatabase);
                AssetDatabase.SaveAssetIfDirty(moveDatabase);
            });
        }
    }

    [Serializable]
    public class MoveData
    {
        public string Id;
        public string Name;
        public Type Type;
        public MoveCategory category;
        [CanBeNull] public int Power;

        public int Accuracy;
        //#TODO public int TotalPP;
        //#TODO Target, enum that specifies who is targeted by the move
        //#TODO FunctionCodes, a list or array of functions;
        //#TODO Flags, a list of enums that are used for cases;
        //#TODO Description, string flavor text summarizing the move;

        public MoveData(PbsEntry pbsEntry)
        {
            try
            {
                Id = pbsEntry.GetId();
                Name = pbsEntry.Lookup("Name");
                Type = Enum.Parse<Type>(pbsEntry.Lookup("Type"));
                category = Enum.Parse<MoveCategory>(pbsEntry.Lookup("Category"));
                Power = int.TryParse(pbsEntry.Lookup("Power"), out Power) ? int.Parse(pbsEntry.Lookup("Power")) : 0;
                Accuracy = int.TryParse(pbsEntry.Lookup("Accuracy"), out Accuracy)
                    ? int.Parse(pbsEntry.Lookup("Accuracy"))
                    : 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while parsing move {pbsEntry.GetId()}");
                Debug.LogException(e);
            }
        }
    }
}