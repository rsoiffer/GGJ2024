using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense
{
    public class Wave : MonoBehaviour
    {
        public int baseLevel;

        public EnemyWeight[] enemies;
        public int numEnemies;
        public float enemySpawnDelay = 1;

        public List<string> encounters;

        public string RandomEnemy()
        {
            var totalWeight = enemies.Sum(e => e.weight);

            var rand = Random.Range(0.0f, totalWeight);
            foreach (var enemy in enemies)
                if (rand < enemy.weight)
                {
                    encounters.Add(enemy.species);
                    return enemy.species;
                }
                else
                {
                    rand -= enemy.weight;
                }

            encounters.Add(enemies[^1].species);
            return enemies[^1].species;
        }

        [Serializable]
        public struct EnemyWeight
        {
            public string species;
            public float weight;
        }
    }
}