﻿using System.Collections.Generic;
using System.Linq;
using Data;
using Helpers;
using JetBrains.Annotations;
using UnityEngine;

namespace TowerDefense
{
    public class PokemonInstance : MonoBehaviour
    {
        public static readonly HashSet<PokemonInstance> AllPokemon = new();

        public PokemonDatabase database;
        public SpriteRenderer sprite;
        public bool isFriendly;
        public int level;
        public int damageTaken;
        public int[] currentStats;
		public float experience;
        public GameObject attackFXPrefab;
		public GameObject fanfarePrefab;


        private PokemonData _data;
        private float _lastPhysicalAttackTime;
        private float _lastSpecialAttackTime;

        private void Update()
        {
            currentStats = new int[6];
            for (var i = 0; i < 6; i++) currentStats[i] = GetStat((Stat)i);
        }

        private void FixedUpdate()
        {
            if (Time.time > _lastPhysicalAttackTime + 1)
            {
                var nearestOther = GetTarget(.75f);
                if (nearestOther != null)
                {
                    _lastPhysicalAttackTime = Time.time;
                    Attack(nearestOther, false);
                }
            }

            if (Time.time > _lastSpecialAttackTime + 2)
            {
                var nearestOther = GetTarget(2);
                if (nearestOther != null)
                {
                    _lastSpecialAttackTime = Time.time;
                    Attack(nearestOther, true);
                }
            }

            if (damageTaken >= GetStat(Stat.HP)){
				if (!isFriendly){
					GameObject controller=GameObject.Find("Controller");
					if (!controller) return;
					EXPManager expManager=controller.GetComponent<EXPManager>();
					expManager.addExp(_data.BaseExp,level);
				}
				Destroy(gameObject);
			}
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

        [CanBeNull]
        private PokemonInstance GetTarget(float maxRange)
        {
            var nearestOther = AllPokemon.Where(p => p.isFriendly != isFriendly)
                .MinByOrElse(p => Vector2.Distance(transform.position, p.transform.position), null);
            if (nearestOther == null) return null;
            var dist = Vector2.Distance(transform.position, nearestOther.transform.position);
            return dist < maxRange ? nearestOther : null;
        }

        public int GetStat(Stat stat)
        {
            if (stat == Stat.HP)
                return Mathf.FloorToInt((level / 100f + 1) * _data.BaseStats[(int)stat] + level);

            return Mathf.FloorToInt((level / 50f + 1) * _data.BaseStats[(int)stat] / 1.5f);
        }

        private void Attack(PokemonInstance target, bool isSpecial)
        {
            var (atk, def) = isSpecial ? (Stat.SPATK, Stat.SPDEF) : (Stat.ATK, Stat.DEF);

            var power = 100;
            var damage = (2 * level / 5f + 2) * power * GetStat(atk) / target.GetStat(def) / 50 + 2;
            damage *= Random.Range(.85f, 1f);
            target.damageTaken += Mathf.CeilToInt(damage);

            var attackFX = Instantiate(attackFXPrefab);
            attackFX.transform.position = target.transform.position;
        }
		
		public void LevelUp(){
			level++;
			var fanfare = Instantiate(fanfarePrefab);
			fanfare.transform.position=transform.position;
		}
		
		public GrowthRate GetGrowthRate(){
			return _data.GrowthRate;
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