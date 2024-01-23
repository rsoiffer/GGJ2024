using System.Collections;
using UnityEngine;

namespace PerfectBidoof
{
    public class Player : MonoBehaviour
    {
        public float speed = 2;
        public float deathTimer = 3;

        public float fireRate = 10;
        public float bulletSpeed = 10;
        public Rigidbody2D bulletPrefab;

        private bool _isDead;
        private float _lastShootTime;
        private Rigidbody2D _rigidbody2d;

        private void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (_isDead)
            {
                _rigidbody2d.velocity = Vector2.zero;
                return;
            }

            var direction = Vector2.zero;
            if (Input.GetKey(KeyCode.W)) direction += Vector2.up;
            if (Input.GetKey(KeyCode.A)) direction += Vector2.left;
            if (Input.GetKey(KeyCode.S)) direction += Vector2.down;
            if (Input.GetKey(KeyCode.D)) direction += Vector2.right;
            direction = Vector2.ClampMagnitude(direction, 1);

            _rigidbody2d.velocity = direction * speed;

            if (Input.GetMouseButton(0) && Time.time > _lastShootTime + 1 / fireRate)
            {
                _lastShootTime = Time.time;
                var newBullet = Instantiate(bulletPrefab);
                newBullet.position = transform.position;
                newBullet.velocity = Vector2.up * bulletSpeed;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy Bullet"))
                StartCoroutine(Death());
        }

        private IEnumerator Death()
        {
            yield return new WaitForSeconds(deathTimer);
            Destroy(gameObject);
        }
    }
}