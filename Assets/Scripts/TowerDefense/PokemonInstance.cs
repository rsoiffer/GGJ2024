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
        public GameObject fanfarePrefab;
        public Attack[] attacks;

        [Header("Instance Data")] public bool isFriendly;
        public int level;
        public PokemonData data;
        public bool isShiny;
        public int experience;
        public List<MoveData> moves;
        public ItemData item;

        [Header("State Data")] public bool inBox;
        public int damageTaken;
        public int[] currentStats;

        private void Update()
        {
            currentStats = new int[6];
            for (var i = 0; i < 6; i++) currentStats[i] = GetStat((Stat)i);
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < attacks.Length; i++)
                attacks[i].move = i < moves.Count ? moves[moves.Count - 1 - i] : null;

            if (inBox) return;

            if (damageTaken >= GetStat(Stat.HP))
            {
                if (!isFriendly)
                {
                    var controller =
                        GameObject.Find("Wave Controller"); // GameObject.Find every frame is REALLY bad practice
                    if (controller)
                    {
                        var expManager = controller.GetComponent<ExpManager>();
                        expManager.AddExp(data.BaseExp, level);
                    }
                }

                Destroy(gameObject);
            }

            // Handle items

            switch (item?.Id)
            {
                case "LEFTOVERS":
                    var regenAmount = .05f * GetStat(Stat.HP) * Time.fixedDeltaTime;
                    damageTaken -= Mathf.FloorToInt(regenAmount + Random.value);
                    if (damageTaken < 0) damageTaken = 0;
                    break;
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

            if (stat == Stat.HP)
                return Mathf.FloorToInt((2 * baseStat + iv) * level / 100f) + level + 10;

            return Mathf.FloorToInt((2 * baseStat + iv) * level / 100f) + 5;

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