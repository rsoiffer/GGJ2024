using Data;
using UnityEngine;

namespace PerfectBidoof
{
    public class Enemy : MonoBehaviour
    {
        public Player player;
        public float speed = .5f;

        public float fireRate = 2;
        public float bulletSpeed = 2;
        public Rigidbody2D bulletPrefab;

        public PokemonData pokemon;
        private float _lastShootTime;

        private Rigidbody2D _rigidbody2d;

        private void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            var sprite = GetComponent<SpriteRenderer>();
            sprite.sprite = pokemon.spriteSet.followers[12];
        }

        private void FixedUpdate()
        {
            if (player == null) return;

            var direction = (player.transform.position - transform.position).normalized;
            _rigidbody2d.velocity = direction * speed;

            if (Time.time > _lastShootTime + 1 / fireRate)
            {
                _lastShootTime = Time.time;
                var newBullet = Instantiate(bulletPrefab);
                newBullet.position = transform.position;
                newBullet.velocity = direction * bulletSpeed;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player Bullet"))
                Destroy(gameObject);
        }
    }
}