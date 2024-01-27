using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class WaveController : MonoBehaviour
    {
        public GridManager gridManager;
        public LaneDefinition[] lanes;

        public EnemyAI enemyPrefab;
		public FriendlyAI friendlyPrefab;
		public Wave[] waves;
		public MoneyManager moneyManager;

        public IEnumerator Start()
        {
            for (var i = 0; i < waves.Length; i++)
            {
                Debug.Log($"Starting Wave {i + 1}");

                for (var j = 0; j < waves[i].numEnemies; j++)
                {
					//int encounters=waves[i].enemies.Length;
                    var lane = lanes[Random.Range(0, lanes.Length)];
                    var enemy = Instantiate(enemyPrefab);
                    enemy.lane = lane;
                    enemy.pokemon.ResetTo(waves[i].randomEnemy(), waves[i].baseLevel);
                    yield return new WaitForSeconds(waves[i].enemySpawnDelay);
                }
				
				/* //Old testing code
				List<string> encounters=waves[i].encounters;
				var ally = Instantiate(friendlyPrefab);
				ally.transform.position=ally.transform.position+new Vector3((i+1)*2.5f,0,0);
				//Debug.Log(encounters[Random.Range(0,encounters.Count)]);
				ally.id=encounters[Random.Range(0,encounters.Count)];
				ally.pokemon.level=waves[i].baseLevel;
				//ally.pokemon.ResetTo("PIDGEY",waves[i].baseLevel);
				team.members.Add(ally.pokemon);
				*/
				
				
				moneyManager.addMoney(100*(i+1));

                yield return new WaitForSeconds(10);
            }
        }
    }

}