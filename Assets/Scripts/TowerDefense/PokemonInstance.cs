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
        [Serializable]
        public enum Status
        {
            Normal,
            Burn,
            Poison,
            Sleep,
            Paralysis,
            Confusion
        }

        public static readonly HashSet<PokemonInstance> AllPokemon = new();

        [Header("References")] public PokemonDatabase pokeDatabase;
        public MoveDatabase moveDatabase;
        public SpriteController sprite;
        public GameObject fanfarePrefab;
        public Attack[] attacks;

        [Header("Instance Data")] public bool isFriendly;
        public int level;
        public PokemonData data;
        public bool isShiny;
        public float experience;

        public Status status = Status.Confusion;
        public int sleeptimer;
        public float statusTime;
        public List<MoveData> moves;
        public ItemData item;


        [Header("State Data")] public bool inBox;
        public int damageTaken;
        public int[] currentStats;
        public GameObject[] statusPrefabs;

        private GameObject statusParticle;

        private void Update()
        {
            currentStats = new int[6];
            for (var i = 0; i < 6; i++) currentStats[i] = GetStat((Stat)i);
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < attacks.Length; i++)
                attacks[i].move = i < moves.Count ? moves[moves.Count - 1 - i] : null;

            if (sleeptimer > 0)
            {
                sleeptimer--;
                if (sleeptimer > 0) return;
                status = Status.Normal;
            }

            if (inBox) return;

            var ai = GetComponent<EnemyAI>();

            //Pokemon can only have 1 status at a time.
            //with the exception of sleep and confusion, statuses are permanant
            var sr = GetComponent<SpriteController>();

            switch (status)
            {
                case Status.Normal:
                    if (statusParticle) Destroy(statusParticle);
                    sr.followerSprite.color = Color.white;
                    if (ai) ai.speed = 1.0f;
                    break;

                case Status.Burn:
                    if (Time.time > statusTime + 2)
                    {
                        damageTaken += Math.Max(GetStat(Stat.HP) / 16, 1);
                        statusTime = Time.time;
                    }

                    sr.followerSprite.color = Color.red;
                    if (!statusParticle) statusParticle = Instantiate(statusPrefabs[1]);
                    break;
                case Status.Poison:
                    if (!statusParticle) statusParticle = Instantiate(statusPrefabs[2]);
                    if (Time.time > statusTime + 2)
                    {
                        damageTaken += Math.Max(GetStat(Stat.HP) / 8, 1);
                        statusTime = Time.time;
                    }

                    sr.followerSprite.color = Color.magenta;
                    break;
                case Status.Sleep:
                    if (ai) ai.speed = 0f;
                    if (!statusParticle) statusParticle = Instantiate(statusPrefabs[3]);
                    sr.followerSprite.color = Color.grey;
                    sleeptimer = 120;
                    break;
                case Status.Paralysis:
                    if (!statusParticle) statusParticle = Instantiate(statusPrefabs[4]);
                    //	statusParticle=Instantiate(paralysisPrefab);
                    if (ai) ai.speed = 0.5f;
                    sr.followerSprite.color = Color.yellow;

                    break;
                case Status.Confusion:
                    if (!statusParticle) statusParticle = Instantiate(statusPrefabs[5]);
                    sr.followerSprite.color = Color.yellow;
                    //Instantiate(ConfusionPrefab);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (statusParticle) statusParticle.transform.position = transform.position;

            if (damageTaken >= GetStat(Stat.HP))
            {
                if (!isFriendly)
                {
                    var controller =
                        GameObject.Find("Wave Controller"); // GameObject.Find every frame is REALLY bad practice
                    if (controller) //It's not every frame, it's called once then destroyed
                    {
                        var expManager = controller.GetComponent<ExpManager>();
                        expManager.AddExp(data.BaseExp, level);
                    }
                }

                Destroy(gameObject);
            }

            if (item?.Id == "LEFTOVERS")
            {
                var regenAmount = .05f * GetStat(Stat.HP) * Time.fixedDeltaTime;
                damageTaken -= Mathf.FloorToInt(regenAmount + Random.value);
                if (damageTaken < 0) damageTaken = 0;
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

        public void OnDestroy()
        {
            if (statusParticle) Destroy(statusParticle);
        }

        public PokemonInstance GetTarget(float maxRange)
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
            var iv = 15;

            var multiplier = 1f;
            if (item?.Id == "QUICKCLAW" && stat == Stat.SPEED && attacks.Any(a => a.LastCritTime > Time.time - 2))
                multiplier *= 2;
            if (item?.Id == "IRONBALL" && stat == Stat.SPEED) multiplier *= .5f;

            if (stat == Stat.HP)
                return Mathf.FloorToInt(multiplier * ((2 * baseStat + iv) * level / 100f + level + 10));

            return Mathf.FloorToInt(multiplier * ((2 * baseStat + iv) * level / 100f + 5));

            // Arceus formula
            /*
            if (stat == Stat.HP)
                return Mathf.FloorToInt((level / 100f + 1) * baseStat + level);

            return Mathf.FloorToInt((level / 50f + 1) * baseStat / 1.5f);
            */
        }

        public void LevelUp()
        {
            level++;

            if (data.Evolutions is { Length: > 0 })
                if (data.Evolutions[1] == "Level")
                {
                    var evoLevel = int.Parse(data.Evolutions[2]);
                    if (level >= evoLevel)
                    {
                        var newId = data.Evolutions[0];
                        data = pokeDatabase.Get(newId);
                    }
                }


            for (var i = 0; i < data.Moves.Length; i++)
                if (data.MoveLearnLevels[i] == level)
                {
                    var move = moveDatabase.Get(data.Moves[i]);
                    if (move.IsValid())
                        moves.Add(move);
                }

            var fanfare = Instantiate(fanfarePrefab);
            fanfare.transform.position = transform.position;
        }

        public GrowthRate GetGrowthRate()
        {
            return data.GrowthRate;
        }

        public void AddExperience(float exp)
        {
            if (item?.Id == "LUCKYEGG") exp *= 1.2f;
            experience += Mathf.FloorToInt(exp + Random.value);
        }

        public void ResetTo(string id, int level, bool autoEvolve = true)
        {
            data = pokeDatabase.Get(id);
            this.level = level;
            experience = Mathf.RoundToInt(ExpManager.MinXpByLevel(this, level));

            if (autoEvolve)
                if (data.Evolutions is { Length: > 0 })
                    if (data.Evolutions[1] == "Level")
                    {
                        var evoLevel = int.Parse(data.Evolutions[2]);
                        if (level >= evoLevel)
                        {
                            var newId = data.Evolutions[0];
                            data = pokeDatabase.Get(newId);
                        }
                    }

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
                transform.position = (Vector2)slot.transform.position;
                sprite.SetToIcon();
            }
            else
            {
                Move(slot.transform.position);
            }
        }
    }
}