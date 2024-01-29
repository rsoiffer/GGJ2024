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
		public GameObject[] statusPrefabs;
		

        [Header("Instance Data")] public bool isFriendly;
        public int level;
        public PokemonData data;
        public bool isShiny;
        public float experience;

		[System.Serializable]
		public enum Status{
			Normal,
			Burn,
			Poison,
			Sleep,
			Paralysis,
			Confusion
		};
		
		public Status status=Status.Confusion;
		public int sleeptimer=0;
		public float statusTime;
		
		private GameObject statusParticle;

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
			if (sleeptimer>0){
				sleeptimer--;
				if (sleeptimer>0) return;
				status=Status.Normal;
			}
            if (inBox) return;

            foreach (var move in moves.Take(2))
            {
                lastMoveUseTimes.TryAdd(move, 0);
                if (Time.time > lastMoveUseTimes[move] + move.Cooldown())
                {

					if (status==Status.Confusion){
						Attack(this, move);
					}
					else{
						var nearestOther = GetTarget(move.Range());
						if (nearestOther != null)
						{
							lastMoveUseTimes[move] = Time.time;
							Attack(nearestOther, move);
						}
					}
                }
			}

                
            
			
				EnemyAI ai=GetComponent<EnemyAI>();
			
			//Pokemon can only have 1 status at a time.
			//with the exception of sleep and confusion, statuses are permanant
			SpriteController sr=GetComponent<SpriteController>();
			
			switch (status){
				
				
				case Status.Normal:
				if (statusParticle){
					Destroy(statusParticle);
				}
					sr.sprite.color=Color.white;
					if (ai){
						ai.speed=1.0f;
					}
				break;
				
				case Status.Burn:
				if (Time.time > statusTime + 2){
						damageTaken+=Math.Max(GetStat(Stat.HP)/16,1);			
						statusTime=Time.time;
					}
					sr.sprite.color=Color.red;
				if (!statusParticle){
					statusParticle=Instantiate(statusPrefabs[1]);
				}
				break;
				case Status.Poison:
				if (!statusParticle){
					statusParticle=Instantiate(statusPrefabs[2]);
				}
					if (Time.time > statusTime + 2){
						damageTaken+=Math.Max(GetStat(Stat.HP)/8,1);			
						statusTime=Time.time;
					}
					sr.sprite.color=Color.magenta;
				break;
				case Status.Sleep:
				if (ai){
					ai.speed=0f;
				}
				if (!statusParticle){
					statusParticle=Instantiate(statusPrefabs[3]);
					
				}
				sr.sprite.color=Color.grey;
				sleeptimer=120;
				break;
				case Status.Paralysis:
				if (!statusParticle){
					statusParticle=Instantiate(statusPrefabs[4]);
				//	statusParticle=Instantiate(paralysisPrefab);
				}
				if (ai){
					ai.speed=0.5f;
				}
				sr.sprite.color=Color.yellow;
			
				break;
				case Status.Confusion:
				if (!statusParticle){
					statusParticle=Instantiate(statusPrefabs[5]);
				}
				sr.sprite.color=Color.yellow;
				//Instantiate(ConfusionPrefab);
				break;
				
			}
			if (statusParticle){
					statusParticle.transform.position = transform.position;
			}

            if (damageTaken >= GetStat(Stat.HP))
            {
                if (!isFriendly)
                {
                    var controller =
                        GameObject.Find("Wave Controller"); // GameObject.Find every frame is REALLY bad practice
                    if (controller)						//It's not every frame, it's called once then destroyed
                    {
                        var expManager = controller.GetComponent<EXPManager>();
                        expManager.addExp(data.BaseExp, level);
                    }
                }

                Destroy(gameObject);
            }
		}
        
		public void OnDestroy(){
			if (statusParticle){
				Destroy(statusParticle);
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

            var power = 100;

            var damage = (2 * level / 5f + 2) * power * GetStat(atk) / target.GetStat(def) / 50 + 2;
			if (status==Status.Burn && move.Category != MoveCategory.Special){
				damage*=0.5f;
			}
            damage *= Random.Range(.85f, 1f);
            target.damageTaken += Mathf.CeilToInt(damage);

            var attackFX = Instantiate(attackFXPrefab);
            attackFX.transform.position = target.transform.position;
			/*
			if (isFriendly){
				switch (Random.Range(1,6)){
					case 1:
					target.status=Status.Burn;
					break;
					case 2:
							target.status=Status.Poison;
					break;
					
					case 3:
							target.status=Status.Sleep;
					break;
					case 4:
					target.status=Status.Paralysis;
					break;
					case 5:
					target.status=Status.Confusion;
					break;
				}
			*/
			
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