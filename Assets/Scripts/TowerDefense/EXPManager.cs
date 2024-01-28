using System.Collections;
using System.Linq;
using Data;
using UnityEngine;

namespace TowerDefense
{
    public class EXPManager : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        //
        public void addExp(int baseExp, int enemyLevel)
        {
            //The formula has all of these values, none should be relevant to this game
            //but I'm putting them here in case
            var a = 1.0f;
            var s = 1.0f;
            var t = 1.0f;
            var e = 1.0f;
            var v = 1.0f;
            var f = 1.0f;
            var p = 1.0f;
            IEnumerable team = PokemonInstance.AllPokemon.Where(p => p.isFriendly);
            foreach (PokemonInstance mon in team)
            {
                mon.experience += baseExp * enemyLevel / 7.0f * a * 1.0f / s * t * e * v * f * p;
                calcLevelUp(mon);
            }
        }

        //For if we want to use EXP candies
        public void addFlatExp(float amount, PokemonInstance mon)
        {
            mon.experience += amount;
            calcLevelUp(mon);
        }


        private void calcLevelUp(PokemonInstance mon)
        {
            var growthrate = mon.GetGrowthRate();
            var nextLevel = mon.level + 1;
            float nextLevelMinExp = 0;
            switch (growthrate)
            {
                //exp=6/5n^3-15n^2+100n-140
                //bulbapedia calls parabolic "medium slow"
                case GrowthRate.Parabolic:
                    nextLevelMinExp = 6.0f / 5.0f * Mathf.Pow(nextLevel, 3.0f) - 15.0f * Mathf.Pow(nextLevel, 2.0f) +
                        100.0f * nextLevel - 140.0f;
                    break;
                //exp=n^3
                //bulbapedia calls medium "medium fast"
                case GrowthRate.Medium:
                    nextLevelMinExp = Mathf.Pow(nextLevel, 3.0f);
                    break;
                //exp=4n^3/5
                case GrowthRate.Fast:
                    nextLevelMinExp = 4.0f / 5.0f * Mathf.Pow(nextLevel, 3.0f);
                    break;
                //exp=5n^3/4
                case GrowthRate.Slow:
                    nextLevelMinExp = 5.0f / 4.0f * Mathf.Pow(nextLevel, 3.0f);
                    break;
                //exp=annoying piecewise function
                case GrowthRate.Fluctuating:
                    if (nextLevel < 15)
                        nextLevelMinExp = Mathf.Pow(nextLevel, 3.0f) * (Mathf.Floor((nextLevel + 1.0f) / 3.0f) + 24) /
                                          50.0f;
                    else if (nextLevel < 36)
                        nextLevelMinExp = Mathf.Pow(nextLevel, 3.0f) * (nextLevel + 14) / 50.0f;
                    else
                        nextLevelMinExp = (Mathf.Pow(nextLevel, 3.0f) * Mathf.Floor(nextLevel / 2) + 32) / 50.0f;
                    break;
                //exp=annoying piecewise function
                case GrowthRate.Erratic:
                    if (nextLevel < 50)
                        nextLevelMinExp = Mathf.Pow(nextLevel, 3.0f) * (100.0f - nextLevel) / 50.0f;
                    else if (nextLevel < 68)
                        nextLevelMinExp = Mathf.Pow(nextLevel, 3.0f) * (150.0f - nextLevel) / 100.0f;
                    else if (nextLevel < 98)
                        nextLevelMinExp = Mathf.Pow(nextLevel, 3.0f) *
                            Mathf.Floor((1911.0f - 10.0f * nextLevel) / 3.0f) / 500.0f;
                    else
                        nextLevelMinExp = Mathf.Pow(nextLevel, 3.0f) * (160.0f - nextLevel) / 100.0f;
                    break;
            }

            if (mon.experience > nextLevelMinExp)
            {
                mon.LevelUp();
                //A single exp drop might level you multiple times
                calcLevelUp(mon);
            }
        }
    }
}