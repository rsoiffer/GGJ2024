using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using AssetProcessing;
#endif

namespace Data
{
    [CreateAssetMenu(fileName = "MoveDatabase", menuName = "Move Database", order = 0)]
    public class MoveDatabase : ScriptableObject
    {
        [SerializeField] private MoveData[] database;

        [CanBeNull] private Dictionary<string, MoveData> _table;

        public MoveData Get(string id)
        {
            if (_table != null) return _table[id];

            _table = new Dictionary<string, MoveData>();
            foreach (var move in database) _table.Add(move.Id, move);
            return _table[id];
        }

        public IEnumerable<MoveData> GetAll()
        {
            return database;
        }

#if UNITY_EDITOR
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
#endif
    }

    [Serializable]
    public class MoveData
    {
        public string Id;
        public string Name;
        public Type Type;
        public MoveCategory Category;
        public int Power;
        public int Accuracy;
        public int TotalPP;
        public string Target;
        public string FunctionCode;
        [CanBeNull] public string[] Flags;
        public int EffectChance;
        public string Description;

#if UNITY_EDITOR
        public MoveData(PbsEntry pbsEntry)
        {
            try
            {
                Id = pbsEntry.GetId();
                Name = pbsEntry.Lookup("Name");
                Type = Enum.Parse<Type>(pbsEntry.Lookup("Type"));
                Category = Enum.Parse<MoveCategory>(pbsEntry.Lookup("Category"));
                Power = int.Parse(pbsEntry.Lookup("Power") ?? "0");
                Accuracy = int.Parse(pbsEntry.Lookup("Accuracy")!);
                TotalPP = int.Parse(pbsEntry.Lookup("TotalPP")!);
                Target = pbsEntry.Lookup("Target");
                FunctionCode = pbsEntry.Lookup("FunctionCode");
                Flags = pbsEntry.Lookup("Flags")?.Split(",");
                EffectChance = int.Parse(pbsEntry.Lookup("EffectChance") ?? "0");
                Description = pbsEntry.Lookup("Description");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while parsing move {pbsEntry.GetId()}");
                Debug.LogException(e);
            }
        }
#endif

        public bool IsValid()
        {
            if (Category == MoveCategory.Status) return false;
            return true;
        }
    }
}