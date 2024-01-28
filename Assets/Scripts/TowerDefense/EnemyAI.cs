using Data;
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
            var speed = Mathf.Pow((pokemon.GetStat(Stat.SPEED) + 20f) / 50f, 1);

            var nearby = pokemon.GetTarget(1);
            if (nearby != null) speed *= .25f;

            laneProgress += Time.fixedDeltaTime * speed;
            transform.position = lane.Position(laneProgress);
        }

        private void LateUpdate()
        {
            var modProgress = laneProgress + Time.time - Time.fixedTime;
            pokemon.Move(lane.Position(modProgress));
        }
    }
}