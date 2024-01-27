using System;
using System.Collections.Generic;
using System.Linq;
using AssetProcessing;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PokemonDatabase", menuName = "Pokemon Database", order = 0)]
    public class PokemonDatabase : ScriptableObject
    {
        public PokemonData[] database;

        [MenuItem("Tools/Build Database")]
        public static void BuildDatabase()
        {
            AssetProcessor.IterateOverSelectedAssets(path =>
            {
                var pokeDatabase = AssetDatabase.LoadAssetAtPath<PokemonDatabase>(path);
                if (pokeDatabase == null) return;
                var pbsDatabase = PbsDatabase.LoadDatabase("pokemon");
                pokeDatabase.database =
                    pbsDatabase.Entries.Values.Select(pbsEntry => new PokemonData(pbsEntry)).ToArray();
                EditorUtility.SetDirty(pokeDatabase);
                AssetDatabase.SaveAssetIfDirty(pokeDatabase);
            });
        }
    }

    [Serializable]
    public class PokemonData
    {
        public string Id;
        [CanBeNull] public string ParentId;
        public string Name;
        [CanBeNull] public string FormName;
        public Type[] Types;
        public int[] BaseStats;
        public GenderRatio GenderRatio;
        public GrowthRate GrowthRate;
        public int BaseExp;
        public string EVs;
        public int CatchRate;
        public int Happiness;
        public string[] Abilities;
        [CanBeNull] public string[] HiddenAbilities;
        public int[] MoveLearnLevels;
        public string[] Moves;
        [CanBeNull] public string[] TutorMoves;
        [CanBeNull] public string[] EggMoves;
        public string[] EggGroups;
        public int HatchSteps;
        public float Height;
        public float Weight;
        public string Color;
        public string Shape;
        public string Habitat;
        public string Category;
        public string Pokedex;
        public int Generation;
        [CanBeNull] public string Evolutions;
        [CanBeNull] public string[] Flags;

        public PokemonSpriteSet spriteSet;
        public PokemonSpriteSet spriteSetShiny;
        public AudioClip cry;

        public PokemonData(PbsEntry pbsEntry)
        {
            try
            {
                Id = pbsEntry.GetId();
                ParentId = pbsEntry.GetParentId();
                Name = pbsEntry.Lookup("Name");
                FormName = pbsEntry.Lookup("FormName");
                Types = pbsEntry.Lookup("Types")!.Split(",").Select(Enum.Parse<Type>).ToArray();
                BaseStats = pbsEntry.Lookup("BaseStats")!.Split(",").Select(int.Parse).ToArray();
                GenderRatio = Enum.Parse<GenderRatio>(pbsEntry.Lookup("GenderRatio"));
                GrowthRate = Enum.Parse<GrowthRate>(pbsEntry.Lookup("GrowthRate"));
                BaseExp = int.Parse(pbsEntry.Lookup("BaseExp")!);
                EVs = pbsEntry.Lookup("EVs");
                CatchRate = int.Parse(pbsEntry.Lookup("CatchRate")!);
                Happiness = int.Parse(pbsEntry.Lookup("Happiness")!);
                Abilities = pbsEntry.Lookup("Abilities")!.Split(",");
                HiddenAbilities = pbsEntry.Lookup("HiddenAbilities")?.Split(",");
                (MoveLearnLevels, Moves) = ParseMoves(pbsEntry.Lookup("Moves")!);
                TutorMoves = pbsEntry.Lookup("TutorMoves")?.Split(",");
                EggMoves = pbsEntry.Lookup("EggMoves")?.Split(",");
                EggGroups = pbsEntry.Lookup("EggGroups")!.Split(",");
                HatchSteps = int.Parse(pbsEntry.Lookup("HatchSteps")!);
                Height = float.Parse(pbsEntry.Lookup("Height")!);
                Weight = float.Parse(pbsEntry.Lookup("Weight")!);
                Color = pbsEntry.Lookup("Color");
                Shape = pbsEntry.Lookup("Shape");
                Habitat = pbsEntry.Lookup("Habitat");
                Category = pbsEntry.Lookup("Category");
                Pokedex = pbsEntry.Lookup("Pokedex");
                Generation = int.Parse(pbsEntry.Lookup("Generation")!);
                Evolutions = pbsEntry.Lookup("Evolutions");
                Flags = pbsEntry.Lookup("Flags")?.Split(",");

                spriteSet = new PokemonSpriteSet(Id, false);
                spriteSetShiny = new PokemonSpriteSet(Id, true);
                cry = AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/DataPack/Audio/SE/Cries/{Id}.ogg");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while parsing pokemon {pbsEntry.GetId()}");
                Debug.LogException(e);
            }
        }

        private static (int[], string[]) ParseMoves(string input)
        {
            var inputData = input.Split(",");
            var moveLearnLevels = new List<int>();
            var moves = new List<string>();
            for (var i = 0; i < inputData.Length; i += 2)
            {
                moveLearnLevels.Add(int.Parse(inputData[i]));
                moves.Add(inputData[i + 1]);
            }

            return (moveLearnLevels.ToArray(), moves.ToArray());
        }
    }

    [Serializable]
    public class PokemonSpriteSet
    {
        public Sprite[] followers;
        public Sprite back;
        public Sprite front;
        public Sprite[] icons;

        public PokemonSpriteSet(string id, bool shiny)
        {
            if (!shiny)
            {
                followers = AssetDatabase.LoadAllAssetsAtPath($"Assets/DataPack/Graphics/Characters/Followers/{id}.png")
                    .OfType<Sprite>().OrderBy(s => s.name).ToArray();
                back = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/DataPack/Graphics/Pokemon/Back/{id}.png");
                front = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/DataPack/Graphics/Pokemon/Front/{id}.png");
                icons = AssetDatabase.LoadAllAssetsAtPath($"Assets/DataPack/Graphics/Pokemon/Icons/{id}.png")
                    .OfType<Sprite>().OrderBy(s => s.name).ToArray();
            }
            else
            {
                followers = AssetDatabase
                    .LoadAllAssetsAtPath($"Assets/DataPack/Graphics/Characters/Followers shiny/{id}.png")
                    .OfType<Sprite>().OrderBy(s => s.name).ToArray();
                back = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/DataPack/Graphics/Pokemon/Back shiny/{id}.png");
                front = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/DataPack/Graphics/Pokemon/Front shiny/{id}.png");
                icons = AssetDatabase.LoadAllAssetsAtPath($"Assets/DataPack/Graphics/Pokemon/Icons shiny/{id}.png")
                    .OfType<Sprite>().OrderBy(s => s.name).ToArray();
            }
        }
    }
}