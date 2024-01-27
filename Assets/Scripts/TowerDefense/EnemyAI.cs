using UnityEngine;

namespace TowerDefense
{
    public class EnemyAI : MonoBehaviour
    {
        public LaneDefinition lane;
        public PokemonInstance pokemon;

        public float laneProgress;

        private void FixedUpdate()
        {
            laneProgress += Time.fixedDeltaTime;
            transform.position = lane.Position(laneProgress);
        }

        private void LateUpdate()
        {
            var modProgress = laneProgress + Time.time - Time.fixedTime;
            pokemon.Move(lane.Position(modProgress));
        }
    }
}