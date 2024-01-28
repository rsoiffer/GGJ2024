using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense
{
    public class SpriteController : MonoBehaviour
    {
        public PokemonInstance pokemon;
        public SpriteRenderer followerSprite;
        public SpriteRenderer shadow;
        public SpriteRenderer iconSprite;
        public SpriteRenderer selectionRing;

        public Vector2 lastDirection;
        public float walkAnimSpeed = 4f;
        public float walkAnimSpeedNoise = .1f;
        public bool isWalking;

        private float _walkAnimSpeedActual;

        public PokemonSpriteSet SpriteSet => pokemon.isShiny ? pokemon.data.spriteSetShiny : pokemon.data.spriteSet;

        private void Awake()
        {
            _walkAnimSpeedActual = walkAnimSpeed + walkAnimSpeedNoise * Random.Range(-1, 1);
        }

        private void LateUpdate()
        {
            if (followerSprite.enabled)
            {
                var spriteId = 0;
                var max = Mathf.Max(Mathf.Abs(lastDirection.x), Mathf.Abs(lastDirection.y));
                if (lastDirection.y >= max) spriteId = 0;
                if (lastDirection.x >= max) spriteId = 4;
                if (-lastDirection.x >= max) spriteId = 8;
                if (-lastDirection.y >= max) spriteId = 12;

                if (isWalking) spriteId += Mathf.FloorToInt(Time.time * _walkAnimSpeedActual) % 4;
                followerSprite.sprite = SpriteSet.followers[spriteId];
            }
        }

        public void Look(Vector2 direction)
        {
            lastDirection = direction;
            followerSprite.enabled = true;
            shadow.enabled = true;
            iconSprite.enabled = false;
        }

        public void SetToIcon()
        {
            iconSprite.sprite = SpriteSet.icons[0];

            followerSprite.enabled = false;
            shadow.enabled = false;
            iconSprite.enabled = true;
        }
    }
}