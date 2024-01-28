using System;
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

        public string RandomEnemy()
        {
            var totalWeight = enemies.Sum(e => e.weight);

            var rand = Random.Range(0, totalWeight);
            foreach (var enemy in enemies)
            {
                rand -= enemy.weight;
                if (rand < 0) return enemy.species;
            }

            throw new InvalidOperationException("Failed to sample a random enemy");
        }

        [Serializable]
        public struct EnemyWeight
        {
            public string species;
            public float weight;
        }
    }
}