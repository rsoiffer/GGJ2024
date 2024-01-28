using System;
using System.Linq;
using Data;
using UnityEngine;

namespace TowerDefense
{
    public class ExpManager : MonoBehaviour
    {
        public void AddExp(int baseExp, int enemyLevel)
        {
            var team = PokemonInstance.AllPokemon.Where(p => p.isFriendly);
            foreach (var mon in team)
            {
                mon.AddExperience(baseExp * enemyLevel / 7.0f);
                CalcLevelUp(mon);
            }
        }

        //For if we want to use EXP candies
        public void AddFlatExp(int amount, PokemonInstance mon)
        {
            mon.experience += amount;
            CalcLevelUp(mon);
        }

        private void CalcLevelUp(PokemonInstance mon)
        {
            var nextLevelMinExp = MinXpByLevel(mon, mon.level + 1);

            if (mon.experience > nextLevelMinExp)
            {
                mon.LevelUp();
                //A single exp drop might level you multiple times
                CalcLevelUp(mon);
            }
        }

        public static float MinXpByLevel(PokemonInstance pokemon, int level)
        {
            switch (pokemon.GetGrowthRate())
            {
                //exp=6/5n^3-15n^2+100n-140
                //bulbapedia calls parabolic "medium slow"
                case GrowthRate.Parabolic:
                    return 6.0f / 5.0f * Mathf.Pow(level, 3.0f) - 15.0f * Mathf.Pow(level, 2.0f) +
                        100.0f * level - 140.0f;
                //exp=n^3
                //bulbapedia calls medium "medium fast"
                case GrowthRate.Medium:
                    return Mathf.Pow(level, 3.0f);
                //exp=4n^3/5
                case GrowthRate.Fast:
                    return 4.0f / 5.0f * Mathf.Pow(level, 3.0f);
                //exp=5n^3/4
                case GrowthRate.Slow:
                    return 5.0f / 4.0f * Mathf.Pow(level, 3.0f);
                //exp=annoying piecewise function
                case GrowthRate.Fluctuating:
                    if (level < 15)
                        return Mathf.Pow(level, 3.0f) * (Mathf.Floor((level + 1.0f) / 3.0f) + 24) /
                               50.0f;
                    if (level < 36)
                        return Mathf.Pow(level, 3.0f) * (level + 14) / 50.0f;
                    return (Mathf.Pow(level, 3.0f) * Mathf.Floor(level / 2f) + 32) / 50.0f;
                //exp=annoying piecewise function
                case GrowthRate.Erratic:
                    if (level < 50)
                        return Mathf.Pow(level, 3.0f) * (100.0f - level) / 50.0f;
                    if (level < 68)
                        return Mathf.Pow(level, 3.0f) * (150.0f - level) / 100.0f;
                    if (level < 98)
                        return Mathf.Pow(level, 3.0f) *
                            Mathf.Floor((1911.0f - 10.0f * level) / 3.0f) / 500.0f;
                    return Mathf.Pow(level, 3.0f) * (160.0f - level) / 100.0f;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}