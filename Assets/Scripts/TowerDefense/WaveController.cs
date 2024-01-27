using System.Collections;
using System.Linq;
using UnityEngine;

namespace TowerDefense
{
    public class WaveController : MonoBehaviour
    {
        public LaneDefinition[] lanes;
        public GameObject rewardUI;

        public FriendlyAI rewardPrefab;
        public EnemyAI enemyPrefab;

        public IEnumerator Start()
        {
            yield return DoReward(0);
            for (var i = 1; i <= 100; i++)
            {
                yield return DoWave(i);
                yield return DoReward(i);
            }
        }

        private IEnumerator DoWave(int waveNum)
        {
            Debug.Log($"Starting Wave {waveNum}");

            for (var j = 0; j < 10; j++)
            {
                var lane = lanes[Random.Range(0, lanes.Length)];
                var enemy = Instantiate(enemyPrefab);
                enemy.lane = lane;
                enemy.pokemon.ResetTo("BIDOOF", 1);
                yield return new WaitForSeconds(1);
            }

            while (PokemonInstance.AllPokemon.Any(p => !p.isFriendly)) yield return null;

            yield return new WaitForSeconds(2);
        }

        private IEnumerator DoReward(int waveNum)
        {
            Debug.Log($"Granting reward for wave {waveNum}");

            rewardUI.SetActive(true);
            var slots = rewardUI.GetComponentsInChildren<Slot>();
            foreach (var slot in slots)
            {
                var reward = Instantiate(rewardPrefab);
                reward.pokemon.ResetTo("PIKACHU", 50);
                reward.pokemon.MoveToSlot(slot);
                slot.Set(reward);
            }

            while (slots.All(s => s.InSlot != null)) yield return null;

            foreach (var slot in slots)
                if (slot.InSlot != null)
                    Destroy(slot.InSlot.gameObject);

            rewardUI.SetActive(false);
        }
    }
}