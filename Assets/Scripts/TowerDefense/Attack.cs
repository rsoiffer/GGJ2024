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

        [Header("Instance Data")] public PokemonInstance pokemon;
        public MoveData move;

        [Header("State Data")] public float lastAttackTime;

        public bool Active => move != null && !string.IsNullOrEmpty(move.Id);

        private void FixedUpdate()
        {
            if (!Active) return;

            if (Time.time > lastAttackTime + Cooldown())
            {
                var nearestOther = pokemon.GetTarget(Range());
                if (nearestOther != null)
                {
                    lastAttackTime = Time.time;
                    DoAttack(nearestOther);
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

        private void DoAttack(PokemonInstance target)
        {
            var (atk, def) = (Atk(), Def(target));

            var damage = (2 * pokemon.level / 5f + 2) * move.Power * atk / def / 50 + 2;
            damage *= Random.Range(.85f, 1f);
            if (pokemon.data.Types.Contains(move.Type)) damage *= 1.5f;
            foreach (var type in target.data.Types)
                damage *= TypeHelpers.GetTypeEffectiveness(move.Type, type);

            if (Random.Range(0, 100f) < move.Accuracy) target.damageTaken += Mathf.FloorToInt(damage);

            var attackFX = Instantiate(fxPrefab);
            attackFX.transform.position = target.transform.position;
            var particles = attackFX.GetComponent<ParticleSystem>();
            var main = particles.main;
            main.startColor = TypeHelpers.TypeColor(move.Type);
        }

        private float Cooldown()
        {
            return (move.Category == MoveCategory.Physical ? 1 : 2) * PowerScale(.5f) *
                   Mathf.Pow((pokemon.GetStat(Stat.SPEED) + 20f) / 50f, -.25f);
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