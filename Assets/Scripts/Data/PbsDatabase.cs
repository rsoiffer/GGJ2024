using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Helpers;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Data
{
    public class PbsDatabase
    {
        private static readonly Dictionary<string, PbsDatabase> Databases = new();

        public readonly Dictionary<string, PbsEntry> Entries = new();

        public static PbsDatabase LoadDatabase(string filter)
        {
            if (Databases.TryGetValue(filter, out var database)) return database;

            var d = new PbsDatabase();
            Databases.Add(filter, d);

            foreach (var path in Directory.GetFiles("Assets/DataPack/PBS"))
                if (path.Contains(filter) && path.EndsWith(".txt"))
                    d.Parse(path);

            return d;
        }

        private void Parse(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            PbsEntry currentEntry = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("#")) continue;

                var match = Regex.Match(line, @"\[(\w+)(,(\d+))?\]");
                if (match.Success)
                {
                    var id = match.Groups[1].Value;
                    string parentId = null;

                    if (match.Groups[3].Success)
                    {
                        parentId = id;
                        id += "_" + match.Groups[3].Value;
                    }

                    if (Entries.TryGetValue(id, out var existingEntry))
                    {
                        currentEntry = existingEntry;
                        continue;
                    }

                    currentEntry = new PbsEntry(this, id, parentId);
                    Entries.Add(currentEntry.GetId(), currentEntry);
                    continue;
                }

                var match2 = Regex.Match(line, @"(\w+) = (.+)");
                if (match2.Success)
                {
                    currentEntry!.AddData(match2.Groups[1].Value, match2.Groups[2].Value);
                    continue;
                }

                Debug.LogWarning($"Failed to parse line: {line}");
            }
        }
    }

    public class PbsEntry
    {
        private readonly Dictionary<string, string> _data = new();
        private readonly PbsDatabase _database;
        private readonly string _id;
        private readonly string _parentID;

        public PbsEntry(PbsDatabase database, string id, [CanBeNull] string parentId)
        {
            _database = database;
            _id = id;
            _parentID = parentId;
        }

        public void AddData(string key, string value)
        {
            _data.Add(key, value);
        }

        public string GetId()
        {
            return _id;
        }

        [CanBeNull]
        public string GetParentId()
        {
            return _parentID;
        }

        [CanBeNull]
        public string Lookup(string key)
        {
            if (_data.TryGetValue(key, out var value)) return value;

            if (_parentID != null)
                if (_database.Entries.TryGetValue(_parentID, out var parentEntry))
                    return parentEntry.Lookup(key);

            return null;
        }
    }

#if UNITY_EDITOR
    public static class PbsDatabaseTests
    {
        [MenuItem("Tools/Tests/Test PBS Database (Pokemon)")]
        public static void TestPokemon()
        {
            var database = PbsDatabase.LoadDatabase("pokemon");
            Debug.Log($"Database has {database.Entries.Count} entries");

            Debug.Log($"There are {database.Entries.Values.Count(pokemon => pokemon.GetParentId() == null)} pokemon" +
                      $" and {database.Entries.Values.Count(pokemon => pokemon.GetParentId() != null)} extra forms");

            var allGenderRatios =
                Enumerable.ToHashSet(database.Entries.Values.SelectMany(p => p.Lookup("GenderRatio")!.Split(",")));
            Debug.Log($"List of gender ratios is {string.Join(",", allGenderRatios)}");

            var allGrowthRates =
                Enumerable.ToHashSet(database.Entries.Values.SelectMany(p => p.Lookup("GrowthRate")!.Split(",")));
            Debug.Log($"List of growth rates is {string.Join(",", allGrowthRates)}");

            var allFlags = Enumerable.ToHashSet(database.Entries.Values
                .SelectMany(p => p.Lookup("Flags") != null ? p.Lookup("Flags")!.Split(",") : new string[] { }));
            Debug.Log($"List of flags is {string.Join(",", allFlags)}");

            var statNames = new[] { "HP", "Atk", "Def", "Speed", "Sp.Atk", "Sp.Def" };
            for (var statId = 0; statId < 6; statId++)
            {
                var statIdCopy = statId;

                var lowest = database.Entries.Values.MinBy(GetStat);
                var highest = database.Entries.Values.MinBy(e => -GetStat(e));
                Debug.Log($"Base {statNames[statId]} ranges from {GetStat(lowest)} ({GetName(lowest)})" +
                          $" to {GetStat(highest)} ({GetName(highest)})");
                continue;

                int GetStat(PbsEntry pokemon)
                {
                    return int.Parse(pokemon.Lookup("BaseStats")!.Split(",")[statIdCopy]);
                }
            }

            for (var statId = 0; statId < 6; statId++)
            {
                var statIdCopy = statId;

                var lowest = database.Entries.Values.MinBy(GetStat);
                var highest = database.Entries.Values.MinBy(e => -GetStat(e));
                Debug.Log($"Level 100 {statNames[statId]} ranges from {GetStat(lowest)} ({GetName(lowest)})" +
                          $" to {GetStat(highest)} ({GetName(highest)})");
                continue;

                int GetStat(PbsEntry pokemon)
                {
                    var data = new PokemonData(pokemon);
                    var level = 100;
                    if ((Stat)statIdCopy == Stat.HP)
                        return Mathf.FloorToInt((level / 100f + 1) * data.BaseStats[statIdCopy] + level);
                    return Mathf.FloorToInt((level / 50f + 1) * data.BaseStats[statIdCopy] / 1.5f);
                }
            }

            string GetName(PbsEntry pokemon)
            {
                return pokemon.Lookup("FormName") ?? pokemon.Lookup("Name");
            }

            var allDatas = database.Entries.Values.Select(p => new PokemonData(p)).ToList();
            var allMoves = new HashSet<string>();
            allMoves.AddRange(allDatas.SelectMany(p => p.Moves));
            allMoves.AddRange(allDatas.SelectMany(p => p.TutorMoves ?? new string[] { }));
            var usageCounts = allMoves.Select(code => (code,
                    allDatas.Count(p => p.Moves.Contains(code) || (p.TutorMoves?.Contains(code) ?? false))))
                .OrderByDescending(x => x.Item2).ToList();

            var databaseMoves = PbsDatabase.LoadDatabase("moves");

            var movesMessage = "";
            foreach (Type type in Enum.GetValues(typeof(Type)))
            {
                movesMessage += $"Type {type}:\n";
                foreach (var (code, usages) in usageCounts)
                    if (usages >= 30)
                        if (databaseMoves.Entries[code].Lookup("Type") == type.ToString())
                            movesMessage += $"{code} used {usages} times\n";
                movesMessage += "\n";
            }

            Debug.Log(movesMessage);
        }

        [MenuItem("Tools/Tests/Test PBS Database (Moves)")]
        public static void TestMoves()
        {
            var database = PbsDatabase.LoadDatabase("moves");
            Debug.Log($"Database has {database.Entries.Count} entries");

            var allFunctionCodes = Enumerable.ToHashSet(database.Entries.Values.Select(p => p.Lookup("FunctionCode")))
                .ToList();
            var usageCounts = allFunctionCodes.Select(code =>
                    (code, database.Entries.Values.Count(p => p.Lookup("FunctionCode") == code)))
                .OrderByDescending(x => x.Item2).ToList();
            var functionCodeMessage = "";
            foreach (var (code, usages) in usageCounts)
                functionCodeMessage += $"{code} used {usages} times\n";
            Debug.Log(functionCodeMessage);

            var allFlags = Enumerable.ToHashSet(database.Entries.Values
                .SelectMany(p => p.Lookup("Flags") != null ? p.Lookup("Flags")!.Split(",") : new string[] { }));
            Debug.Log($"List of flags is {string.Join(",", allFlags)}");

            var allTargets = Enumerable.ToHashSet(database.Entries.Values.Select(p => p.Lookup("Target")))
                .ToList();
            var usageCounts2 = allTargets.Select(code =>
                    (code, database.Entries.Values.Count(p => p.Lookup("Target") == code)))
                .OrderByDescending(x => x.Item2).ToList();
            var targetMessage = "";
            foreach (var (code, usages) in usageCounts2)
                targetMessage += $"{code} used {usages} times\n";
            Debug.Log(targetMessage);
        }
    }
#endif
}