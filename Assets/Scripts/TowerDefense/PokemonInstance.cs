using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Helpers;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense
{
    public class PokemonInstance : MonoBehaviour
    {
        public static readonly HashSet<PokemonInstance> AllPokemon = new();

        [Header("References")] public PokemonDatabase pokeDatabase;
        public MoveDatabase moveDatabase;
        public SpriteController sprite;
        public GameObject attackFXPrefab;
        public GameObject fanfarePrefab;

        [Header("Instance Data")] public bool isFriendly;
        public int level;
        public PokemonData data;
        public bool isShiny;
        public float experience;
        public List<MoveData> moves;

        [Header("State Data")] public bool inBox;
        public int damageTaken;
        public int[] currentStats;
        public Dictionary<MoveData, float> lastMoveUseTimes = new();

        private void Update()
        {
            currentStats = new int[6];
            for (var i = 0; i < 6; i++) currentStats[i] = GetStat((Stat)i);
        }

        private void FixedUpdate()
        {
            if (inBox) return;

            foreach (var move in moves.Take(4))
            {
                lastMoveUseTimes.TryAdd(move, 0);
                if (Time.time > lastMoveUseTimes[move] + move.Cooldown())
                {
                    var nearestOther = GetTarget(move.Range());
                    if (nearestOther != null)
                    {
                        lastMoveUseTimes[move] = Time.time;
                        Attack(nearestOther, move);
                    }
                }
            }

            if (damageTaken >= GetStat(Stat.HP))
            {
                if (!isFriendly)
                {
                    var controller =
                        GameObject.Find("Wave Controller"); // GameObject.Find every frame is REALLY bad practice
                    if (controller)
                    {
                        var expManager = controller.GetComponent<EXPManager>();
                        expManager.addExp(data.BaseExp, level);
                    }
                }

                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            AllPokemon.Add(this);
        }

        private void OnDisable()
        {
            AllPokemon.Remove(this);
        }

        [CanBeNull]
        private PokemonInstance GetTarget(float maxRange)
        {
            return GetNearest(transform.position, maxRange, p => p.isFriendly != isFriendly);
        }

        [CanBeNull]
        public static PokemonInstance GetNearest(Vector2 position, float maxRange, Func<PokemonInstance, bool> filter)
        {
            var nearestOther = AllPokemon.Where(filter)
                .MinByOrElse(p => Vector2.Distance(position, p.transform.position), null);
            if (nearestOther == null) return null;
            var dist = Vector2.Distance(position, nearestOther.transform.position);
            return dist < maxRange ? nearestOther : null;
        }

        public int GetStat(Stat stat)
        {
            var baseStat = data.BaseStats[(int)stat];

            if (stat == Stat.HP)
                return Mathf.FloorToInt(2 * baseStat * level / 100f) + level + 10;

            return Mathf.FloorToInt(2 * baseStat * level / 100f) + 5;

            // Arceus formula
            /*
            if (stat == Stat.HP)
                return Mathf.FloorToInt((level / 100f + 1) * baseStat + level);

            return Mathf.FloorToInt((level / 50f + 1) * baseStat / 1.5f);
            */
        }

        private void Attack(PokemonInstance target, MoveData move)
        {
            var (atk, def) = move.Category == MoveCategory.Special ? (Stat.SPATK, Stat.SPDEF) : (Stat.ATK, Stat.DEF);

            var damage = (2 * level / 5f + 2) * move.Power * GetStat(atk) / target.GetStat(def) / 50 + 2;
            damage *= Random.Range(.85f, 1f);
            if (data.Types.Contains(move.Type)) damage *= 1.5f;
            foreach (var type in target.data.Types)
                damage *= TypeEffectiveness.GetTypeEffectiveness(move.Type, type);
            target.damageTaken += Mathf.CeilToInt(damage);

            var attackFX = Instantiate(attackFXPrefab);
            attackFX.transform.position = target.transform.position;
        }

        public void LevelUp()
        {
            level++;
            var fanfare = Instantiate(fanfarePrefab);
            fanfare.transform.position = transform.position;
        }

        public GrowthRate GetGrowthRate()
        {
            return data.GrowthRate;
        }

        public void ResetTo(string id, int level)
        {
            data = pokeDatabase.Get(id);
            this.level = level;
            moves.Clear();
            for (var i = 0; i < data.Moves.Length; i++)
                if (data.MoveLearnLevels[i] <= level)
                {
                    var move = moveDatabase.Get(data.Moves[i]);
                    if (move.IsValid())
                        moves.Add(move);
                }
        }

        public void Move(Vector2 pos)
        {
            var direction = pos - (Vector2)transform.position;
            transform.position = pos;
            sprite.Look(direction);
        }

        public void MoveToSlot(Slot slot)
        {
            inBox = slot.isBox;
            if (slot.isBox)
            {
                transform.position = slot.transform.position;
                sprite.SetToIcon();
            }
            else
            {
                Move(slot.transform.position);
            }
        }
    }
}