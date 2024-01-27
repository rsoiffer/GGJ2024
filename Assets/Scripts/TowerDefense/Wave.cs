using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense
{
    public class Wave : MonoBehaviour
    {
        public int baseLevel;

        public enemyWeight[] enemies;
        public int numEnemies;
        public float enemySpawnDelay = 1;

        public List<string> encounters;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public string randomEnemy()
        {
            float totalWeight = 0;
            for (var i = 0; i < enemies.Length; i++) totalWeight += enemies[i].weight;
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

            encounters.Add(enemies[enemies.Length - 1].species);
            return enemies[enemies.Length - 1].species;
        }

        [Serializable]
        public struct enemyWeight
        {
            public string species;
            public float weight;
        }
    }
}