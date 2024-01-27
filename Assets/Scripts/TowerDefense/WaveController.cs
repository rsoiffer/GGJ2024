using System.Collections;
using UnityEngine;

namespace TowerDefense
{
    public class WaveController : MonoBehaviour
    {
        public LaneDefinition[] lanes;

        public EnemyAI enemyPrefab;

        public IEnumerator Start()
        {
            for (var i = 0; i < 100; i++)
            {
                Debug.Log($"Starting Wave {i + 1}");

                for (var j = 0; j < 10; j++)
                {
                    var lane = lanes[Random.Range(0, lanes.Length)];
                    var enemy = Instantiate(enemyPrefab);
                    enemy.lane = lane;
                    enemy.pokemon.ResetTo("BIDOOF", 1);
                    yield return new WaitForSeconds(1);
                }

                yield return new WaitForSeconds(10);
            }
        }
    }
}