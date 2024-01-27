using UnityEngine;

namespace TowerDefense
{
    public class EnemyAI : MonoBehaviour
    {
        public LaneDefinition lane;
        public PokemonInstance pokemon;
		//public Wave[] waves;

        public float laneProgress;

        private void Update()
        {
            var modProgress = laneProgress + Time.time - Time.fixedTime;
            pokemon.Move(lane.Position(modProgress));
        }

        private void FixedUpdate()
        {
            laneProgress += Time.fixedDeltaTime;
            transform.position = lane.Position(laneProgress);
        }
    }
}