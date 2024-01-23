using System.Collections;
using Data;
using UnityEngine;

namespace PerfectBidoof
{
    public class PerfectBidoofController : MonoBehaviour
    {
        public Player player;

        public Enemy enemyPrefab;
        public float enemySpawnRate = 1;
        public float enemySpawnRateScaling = .1f;

        public Vector2 spawnPosMin;
        public Vector2 spawnPosMax;

        private IEnumerator Start()
        {
            var database = PbsDatabase.LoadDatabase("pokemon_base");

            var startTime = Time.time;
            foreach (var p in database.Entries.Values)
            {
                var currentTime = Time.time;
                var spawnRate = enemySpawnRate + enemySpawnRateScaling * (currentTime - startTime);
                yield return new WaitForSeconds(1 / spawnRate);

                var newEnemy = Instantiate(enemyPrefab);
                newEnemy.transform.position = new Vector2(Random.Range(spawnPosMin.x, spawnPosMax.x),
                    Random.Range(spawnPosMin.y, spawnPosMax.y));
                newEnemy.pokemon = new PokemonData(p);
                newEnemy.player = player;
            }
        }
    }
}