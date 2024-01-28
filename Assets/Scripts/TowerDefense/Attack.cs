using System.Linq;
using Data;
using Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense
{
    public class Attack : MonoBehaviour
    {
        [Header("References")] public SpriteRenderer sprite;
        public GameObject fxPrefab;
        public Projectile projectilePrefab;

        [Header("Instance Data")] public PokemonInstance pokemon;
        public MoveData move;

        [Header("State Data")] private float _lastAttackTime = -100;
        public float LastCritTime { get; private set; } = -100;

        public bool Active => move != null && !string.IsNullOrEmpty(move.Id);

        private void FixedUpdate()
        {
            if (!Active) return;

            if (CooldownRemaining() == 0)
            {
                var nearestOther = pokemon.GetTarget(Range());
                if (nearestOther != null)
                {
                    _lastAttackTime = Time.time;
                    if (move.Category == MoveCategory.Special)
                    {
                        var projectile = Instantiate(projectilePrefab);
                        projectile.SetTarget(this, nearestOther);
                    }
                    else
                    {
                        Hit(nearestOther);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (!Active || SelectionController.Instance.selected != pokemon)
            {
                sprite.enabled = false;
                return;
            }

            sprite.enabled = true;
            sprite.color = TypeHelpers.TypeColor(move.Type).WithA(.4f);
            sprite.transform.localScale = .3f * Range() * Vector3.one;
        }

        public float CooldownRemaining()
        {
            return Mathf.Clamp01(1 - (Time.time - _lastAttackTime) / Cooldown());
        }

        public void Hit(PokemonInstance target)
        {
            var (atk, def) = (Atk(), Def(target));

            var damage = (2 * pokemon.level / 5f + 2) * move.Power * atk / def / 50 + 2;
            damage *= Random.Range(.85f, 1f);
            if (pokemon.data.Types.Contains(move.Type)) damage *= 1.5f;
            foreach (var type in target.data.Types)
                damage *= TypeHelpers.GetTypeEffectiveness(move.Type, type);

            var isCrit = Random.value < 1 / 24f;
            if (isCrit)
            {
                damage *= 1.5f;
                LastCritTime = Time.time;
            }

            damage *= .25f; // GLOBAL DAMAGE MULTIPLIER

            if (Random.Range(0, 100f) < move.Accuracy)
            {
                target.damageTaken += Mathf.FloorToInt(damage + Random.value);

                if (pokemon.item?.Id == "SHELLBELL" && move.Category == MoveCategory.Physical)
                {
                    pokemon.damageTaken -= Mathf.FloorToInt(damage / 8 + Random.value);
                    if (pokemon.damageTaken < 0) pokemon.damageTaken = 0;
                }
            }

            var attackFX = Instantiate(fxPrefab);
            attackFX.transform.position = target.transform.position;
            var particles = attackFX.GetComponent<ParticleSystem>();
            var main = particles.main;
            main.startColor = TypeHelpers.TypeColor(move.Type);
        }

        private float Cooldown()
        {
            return (move.Category == MoveCategory.Physical ? 1 : 2) * PowerScale(.5f) *
                   Mathf.Pow((pokemon.GetStat(Stat.SPEED) + 20f) / 50f, -1);
        }

        private float Range()
        {
            return (move.Category == MoveCategory.Physical ? .75f : 2.5f) * PowerScale(.5f) *
                   Mathf.Pow((Atk() + 20f) / 50f, .25f);
        }

        private float PowerScale(float exp)
        {
            return Mathf.Pow(20f / move.TotalPP, exp);
        }

        private int Atk()
        {
            return pokemon.GetStat(move.Category == MoveCategory.Special ? Stat.SPATK : Stat.ATK);
        }

        private int Def(PokemonInstance target)
        {
            return target.GetStat(move.Category == MoveCategory.Special ? Stat.SPATK : Stat.ATK);
        }
    }
}