using System.Collections.Generic;
using System.Linq;
using Data;
using Helpers;
using UnityEngine;

namespace TowerDefense
{
    public class PokemonInstance : MonoBehaviour
    {
        private static readonly HashSet<PokemonInstance> AllPokemon = new();

        public PokemonDatabase database;
        public SpriteRenderer sprite;
        public bool isFriendly;
        public int level;
        public int damageTaken;
        public int[] currentStats;

        private PokemonData _data;
        private float _lastAttackTime;

        private void Update()
        {
            currentStats = new int[6];
            for (var i = 0; i < 6; i++) currentStats[i] = GetStat((Stat)i);
        }

        private void FixedUpdate()
        {
            if (Time.time > _lastAttackTime + 1)
            {
                var nearestOther = AllPokemon.Where(p => p.isFriendly != isFriendly)
                    .MinByOrElse(p => Vector2.Distance(transform.position, p.transform.position), null);
                if (nearestOther != null)
                {
                    var dist = Vector2.Distance(transform.position, nearestOther.transform.position);
                    if (dist < 1)
                    {
                        _lastAttackTime = Time.time;
                        Attack(nearestOther);
                    }
                }
            }

            if (damageTaken >= GetStat(Stat.HP)) Destroy(gameObject);
        }

        private void OnEnable()
        {
            AllPokemon.Add(this);
            sprite.enabled = false;
        }

        private void OnDisable()
        {
            AllPokemon.Remove(this);
        }

        private int GetStat(Stat stat)
        {
            if (stat == Stat.HP)
                return Mathf.FloorToInt((level / 100f + 1) * _data.BaseStats[(int)stat] + level);

            return Mathf.FloorToInt((level / 50f + 1) * _data.BaseStats[(int)stat] / 1.5f);
        }

        private void Attack(PokemonInstance target)
        {
            var power = 100;
            var damage = (2 * level / 5f + 2) * power * GetStat(Stat.ATK) / target.GetStat(Stat.DEF) / 50 + 2;
            damage *= Random.Range(.85f, 1f);
            target.damageTaken += Mathf.CeilToInt(damage);
        }

        public void ResetTo(string id, int level)
        {
            _data = database.database.First(p => p.Id == id);
            this.level = level;
        }

        public void Move(Vector2 pos)
        {
            var direction = pos - (Vector2)transform.position;
            transform.position = pos;
            // if (direction.magnitude < 1e-3f) return;
            sprite.enabled = true;
            var max = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
            if (direction.y >= max) sprite.sprite = _data.spriteSet.followers[0];
            if (direction.x >= max) sprite.sprite = _data.spriteSet.followers[4];
            if (-direction.x >= max) sprite.sprite = _data.spriteSet.followers[8];
            if (-direction.y >= max) sprite.sprite = _data.spriteSet.followers[12];
        }
    }
}