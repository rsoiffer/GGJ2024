using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
	public int baseLevel;
		[System.Serializable]
	public struct enemyWeight{
		public string species;
		public float weight;
	};

	public enemyWeight[] enemies;
	public int numEnemies;
	public float enemySpawnDelay=1;
	public List<string> encounters;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public string randomEnemy(){
		float totalWeight=0;
		for (int i=0; i<enemies.Length; i++){
			totalWeight+=enemies[i].weight;
		}
		float rand=Random.Range(0.0f,totalWeight);
		foreach (enemyWeight enemy in enemies){
		if (rand<enemy.weight){
			encounters.Add(enemy.species);
				return enemy.species;	
		}
		else{
			rand-=enemy.weight;
		}
	}
				encounters.Add(enemies[enemies.Length-1].species);
	return enemies[enemies.Length-1].species;
}

}

