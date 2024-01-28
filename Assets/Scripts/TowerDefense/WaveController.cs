using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense
{
    public class WaveController : MonoBehaviour
    {
        public LaneDefinition[] lanes;
        public GameObject rewardUI;
        public int starterLevel = 10;

        public EnemyAI enemyPrefab;
        public FriendlyAI friendlyPrefab;
        public Wave[] waves;
        public MoneyManager moneyManager;

        private readonly List<(string id, int level)> encounters = new();

        public IEnumerator Start()
        {
            yield return DoReward(-1);

            for (var i = 0; i < 100; i++)
            {
                yield return DoWave(i);
                yield return DoReward(i);
            }
        }

        private IEnumerator DoWave(int waveNum)
        {
            Debug.Log($"Starting Wave {waveNum + 1}");
            encounters.Clear();

            var wave = waves[Mathf.Clamp(waveNum, 0, waves.Length - 1)];
            var levelBonus = 0;
            if (waveNum >= waves.Length) levelBonus = waveNum - waves.Length + 1;

            for (var j = 0; j < wave.numEnemies; j++)
            {
                var lane = lanes[Random.Range(0, lanes.Length)];
                var enemy = Instantiate(enemyPrefab);
                enemy.lane = lane;
                enemy.pokemon.ResetTo(wave.RandomEnemy(), wave.baseLevel + levelBonus);
                encounters.Add((enemy.pokemon.data.Id, enemy.pokemon.level));
                yield return new WaitForSeconds(wave.enemySpawnDelay);
            }

            while (PokemonInstance.AllPokemon.Any(p => !p.isFriendly)) yield return null;

            yield return new WaitForSeconds(2);

            moneyManager.addMoney(100 * (waveNum + 1));
        }

        private IEnumerator DoReward(int waveNum)
        {
            Debug.Log($"Granting reward for wave {waveNum}");

            foreach (var p in PokemonInstance.AllPokemon) p.damageTaken = 0;

            rewardUI.SetActive(true);
            var slots = rewardUI.GetComponentsInChildren<Slot>();
            for (var i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                var reward = Instantiate(friendlyPrefab);
                SetReward(reward.pokemon, waveNum, i);
                reward.pokemon.MoveToSlot(slot);
                slot.Set(reward);
            }

            while (slots.All(s => s.InSlot != null)) yield return null;

            foreach (var slot in slots)
                if (slot.InSlot != null)
                    Destroy(slot.InSlot.gameObject);

            rewardUI.SetActive(false);
        }

        private void SetReward(PokemonInstance pokemon, int waveNum, int num)
        {
            if (waveNum == -1)
            {
                var starterList = num switch
                {
                    0 => new[]
                    {
                        "BULBASAUR", "CHIKORITA", "TREECKO", "TURTWIG", "SNIVY", "CHESPIN", "ROWLET", "GROOKEY",
                        "SPRIGATITO"
                    },
                    1 => new[]
                    {
                        "CHARMANDER", "CYNDAQUIL", "TORCHIC", "CHIMCHAR", "TEPIG", "FENNEKIN", "LITTEN", "SCORBUNNY",
                        "FUECOCO"
                    },
                    2 => new[]
                    {
                        "SQUIRTLE", "TOTODILE", "MUDKIP", "PIPLUP", "OSHAWOTT", "FROAKIE", "POPPLIO", "SOBBLE", "QUAXLY"
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(num), num, null)
                };
                pokemon.ResetTo(starterList[Random.Range(0, starterList.Length)], starterLevel);
            }
            else
            {
                var randomEncounter = encounters[Random.Range(0, encounters.Count)];
                pokemon.ResetTo(randomEncounter.id, randomEncounter.level);
            }
        }
    }
}