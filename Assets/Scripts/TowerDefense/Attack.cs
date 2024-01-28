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

        private void FixedUpdate()
        {
            if (move == null || string.IsNullOrEmpty(move.Id)) return;

            if (Time.time > lastAttackTime + move.Cooldown())
            {
                var nearestOther = pokemon.GetTarget(move.Range());
                if (nearestOther != null)
                {
                    lastAttackTime = Time.time;
                    DoAttack(nearestOther);
                }
            }
        }

        private void LateUpdate()
        {
            if (move == null || string.IsNullOrEmpty(move.Id) || SelectionController.Instance.selected != pokemon)
            {
                sprite.enabled = false;
                return;
            }

            sprite.enabled = true;
            sprite.color = TypeHelpers.TypeColor(move.Type).WithA(.2f);
            sprite.transform.localScale = 2.3f * move.Range() * Vector3.one;
        }

        private void DoAttack(PokemonInstance target)
        {
            var (atkStat, defStat) =
                move.Category == MoveCategory.Special ? (Stat.SPATK, Stat.SPDEF) : (Stat.ATK, Stat.DEF);
            var atk = pokemon.GetStat(atkStat);
            var def = target.GetStat(defStat);

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
    }
}