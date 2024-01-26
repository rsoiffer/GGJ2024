using System;
using System.Collections.Generic;
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
                var pokeDatabase = AssetDatabase.LoadAssetAtPath<PokemonDatabase>(path);
                if (pokeDatabase == null) return;
                var pbsDatabase = PbsDatabase.LoadDatabase("moves");
                pokeDatabase.database =
                    pbsDatabase.Entries.Values.Select(pbsEntry => new PokemonData(pbsEntry)).ToArray();
                EditorUtility.SetDirty(pokeDatabase);
                AssetDatabase.SaveAssetIfDirty(pokeDatabase);
            });
        }
    }

    [Serializable]
    public class MoveData
    {
        public string Name;
        public Type Type;
        public MoveCategory category;
        public int Power;

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
                Name = pbsEntry.Lookup("Name");
                Type = Enum.Parse<Type>(pbsEntry.Lookup("Type"));
                category = Enum.Parse<MoveCategory>(pbsEntry.Lookup("Category"));
                Power = int.Parse(pbsEntry.Lookup("Power")) != null ? int.Parse(pbsEntry.Lookup("Power")) : 0;
                Accuracy = int.Parse(pbsEntry.Lookup("Accuracy"));


            }
            catch (Exception e)
            {
                Debug.LogError($"Error while parsing move {pbsEntry.GetId()}");
                Debug.LogException(e);
            }
        }
    }
}
