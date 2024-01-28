using UnityEngine;

namespace TowerDefense
{
    public class EnemyAI : MonoBehaviour
    {
        public LaneDefinition lane;

        public PokemonInstance pokemon;
		public float speed=1;
        //public Wave[] waves;

        public float laneProgress;

        private void FixedUpdate()
        {
            laneProgress += Time.fixedDeltaTime*speed;
            transform.position = lane.Position(laneProgress);
        }

        private void LateUpdate()
        {
            var modProgress = laneProgress + (Time.time - Time.fixedTime)*speed;
            pokemon.Move(lane.Position(modProgress));
        }
    }
}