using Data;
using UnityEngine;

namespace TowerDefense
{
    public class SpriteController : MonoBehaviour
    {
        public PokemonInstance pokemon;
        public SpriteRenderer followerSprite;
        public SpriteRenderer shadow;
        public SpriteRenderer iconSprite;
        public SpriteRenderer selectionRing;

        public PokemonSpriteSet SpriteSet => pokemon.isShiny ? pokemon.data.spriteSetShiny : pokemon.data.spriteSet;

        public void Look(Vector2 direction)
        {
            var max = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
            if (direction.y >= max) followerSprite.sprite = SpriteSet.followers[0];
            if (direction.x >= max) followerSprite.sprite = SpriteSet.followers[4];
            if (-direction.x >= max) followerSprite.sprite = SpriteSet.followers[8];
            if (-direction.y >= max) followerSprite.sprite = SpriteSet.followers[12];

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