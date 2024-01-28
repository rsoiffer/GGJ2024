using UnityEngine;

namespace TowerDefense
{
    public class Projectile : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public float speed = 10;

        private Attack _attack;
        private float _hitTime;
        private Vector2 _startPos;
        private float _startTime;
        private PokemonInstance _target;

        private void FixedUpdate()
        {
            if (_attack == null || _target == null)
            {
                Destroy(gameObject);
                return;
            }

            if (Time.time > _hitTime)
            {
                _attack.Hit(_target);
                Destroy(gameObject);
            }
        }

        private void LateUpdate()
        {
            if (_attack == null || _target == null)
            {
                Destroy(gameObject);
                return;
            }

            var lerp = Mathf.InverseLerp(_startTime, _hitTime, Time.time);
            transform.position = Vector2.Lerp(_startPos, _target.transform.position, lerp);
        }

        public void SetTarget(Attack attack, PokemonInstance target)
        {
            _attack = attack;
            _target = target;
            _startPos = _attack.transform.position;
            _startTime = Time.time;
            _hitTime = _startTime + Vector2.Distance(_startPos, target.transform.position) / speed;

            spriteRenderer.color = TypeHelpers.TypeColor(_attack.move.Type);
        }
    }
}