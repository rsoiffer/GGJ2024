using Data;
using UnityEngine;

namespace TowerDefense
{
    public class EnemyAI : MonoBehaviour
    {
        public LaneDefinition lane;
        public PokemonInstance pokemon;

        public float speed;
        public float laneProgress;

        private void FixedUpdate()
        {
            var currentSpeed = speed * Mathf.Pow((pokemon.GetStat(Stat.SPEED) + 20f) / 50f, 1);

            var nearby = pokemon.GetTarget(1);
            if (nearby != null) currentSpeed *= .25f;

            laneProgress += Time.fixedDeltaTime * currentSpeed;
            transform.position = lane.Position(laneProgress);
        }

        private void LateUpdate()
        {
            var currentSpeed = speed * Mathf.Pow((pokemon.GetStat(Stat.SPEED) + 20f) / 50f, 1);

            var modProgress = laneProgress + (Time.time - Time.fixedTime) * currentSpeed;
            pokemon.Move(lane.Position(modProgress));
        }
    }
}