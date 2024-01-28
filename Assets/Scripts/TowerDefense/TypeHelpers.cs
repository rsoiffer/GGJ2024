using System;
using System.Collections.Generic;
using UnityEngine;
using static Data.Type;
using Type = Data.Type;

namespace TowerDefense
{
    public static class TypeHelpers
    {
        private static readonly Type[] Types =
        {
            NORMAL, FIGHTING, FLYING, POISON, GROUND, ROCK, BUG, GHOST, STEEL, FIRE, WATER, GRASS, ELECTRIC,
            PSYCHIC, ICE, DRAGON, DARK, FAIRY
        };

        private static readonly float[][] TypeData =
        {
            new[] { 1, 1, 1, 1, 1, 0.5f, 1, 0, 0.5f, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            new[] { 2, 1, 0.5f, 0.5f, 1, 2, 0.5f, 0, 2, 1, 1, 1, 1, 0.5f, 2, 1, 2, 0.5f },
            new[] { 1, 2, 1, 1, 1, 0.5f, 2, 1, 0.5f, 1, 1, 2, 0.5f, 1, 1, 1, 1, 1 },
            new[] { 1, 1, 1, 0.5f, 0.5f, 0.5f, 1, 0.5f, 0, 1, 1, 2, 1, 1, 1, 1, 1, 2 },
            new[] { 1, 1, 0, 2, 1, 2, 0.5f, 1, 2, 2, 1, 0.5f, 2, 1, 1, 1, 1, 1 },
            new[] { 1, 0.5f, 2, 1, 0.5f, 1, 2, 1, 0.5f, 2, 1, 1, 1, 1, 2, 1, 1, 1 },
            new[] { 1, 0.5f, 0.5f, 0.5f, 1, 1, 1, 0.5f, 0.5f, 0.5f, 1, 2, 1, 2, 1, 1, 2, 0.5f },
            new[] { 0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5f, 1 },
            new[] { 1, 1, 1, 1, 1, 2, 1, 1, 0.5f, 0.5f, 0.5f, 1, 0.5f, 1, 2, 1, 1, 2 },
            new[] { 1, 1, 1, 1, 1, 0.5f, 2, 1, 2, 0.5f, 0.5f, 2, 1, 1, 2, 0.5f, 1, 1 },
            new[] { 1, 1, 1, 1, 2, 2, 1, 1, 1, 2, 0.5f, 0.5f, 1, 1, 1, 0.5f, 1, 1 },
            new[] { 1, 1, 0.5f, 0.5f, 2, 2, 0.5f, 1, 0.5f, 0.5f, 2, 0.5f, 1, 1, 1, 0.5f, 1, 1 },
            new[] { 1, 1, 2, 1, 0, 1, 1, 1, 1, 1, 2, 0.5f, 0.5f, 1, 1, 0.5f, 1, 1 },
            new[] { 1, 2, 1, 2, 1, 1, 1, 1, 0.5f, 1, 1, 1, 1, 0.5f, 1, 1, 0, 1 },
            new[] { 1, 1, 2, 1, 2, 1, 1, 1, 0.5f, 0.5f, 0.5f, 2, 1, 1, 0.5f, 2, 1, 1 },
            new[] { 1, 1, 1, 1, 1, 1, 1, 1, 0.5f, 1, 1, 1, 1, 1, 1, 2, 1, 0 },
            new[] { 1, 0.5f, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5f, 0.5f },
            new[] { 1, 2, 1, 0.5f, 1, 1, 1, 1, 0.5f, 0.5f, 1, 1, 1, 1, 1, 2, 2, 1 }
        };

        private static readonly Dictionary<Type, Color> TypeColors = new()
        {
            { NORMAL, ToColor("#A8A77A") },
            { FIRE, ToColor("#EE8130") },
            { WATER, ToColor("#6390F0") },
            { ELECTRIC, ToColor("#F7D02C") },
            { GRASS, ToColor("#7AC74C") },
            { ICE, ToColor("#96D9D6") },
            { FIGHTING, ToColor("#C22E28") },
            { POISON, ToColor("#A33EA1") },
            { GROUND, ToColor("#E2BF65") },
            { FLYING, ToColor("#A98FF3") },
            { PSYCHIC, ToColor("#F95587") },
            { BUG, ToColor("#A6B91A") },
            { ROCK, ToColor("#B6A136") },
            { GHOST, ToColor("#735797") },
            { DRAGON, ToColor("#6F35FC") },
            { DARK, ToColor("#705746") },
            { STEEL, ToColor("#B7B7CE") },
            { FAIRY, ToColor("#D685AD") }
        };

        public static float GetTypeEffectiveness(Type attacker, Type defender)
        {
            return TypeData[Array.IndexOf(Types, attacker)][Array.IndexOf(Types, defender)];
        }

        private static Color ToColor(string s)
        {
            ColorUtility.TryParseHtmlString(s, out var color);
            Debug.Log($"Color {s} parsed to {color}");
            return color;
        }

        public static Color TypeColor(Type type)
        {
            return TypeColors[type];
        }
    }
}