using Data;
using UnityEngine;

namespace TowerDefense
{
    public class SpriteController : MonoBehaviour
    {
        public PokemonInstance pokemon;
        public SpriteRenderer sprite;
        public SpriteRenderer selectionRing;

        public PokemonSpriteSet SpriteSet => pokemon.isShiny ? pokemon.data.spriteSetShiny : pokemon.data.spriteSet;

        public void Look(Vector2 direction)
        {
            var max = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
            if (direction.y >= max) sprite.sprite = SpriteSet.followers[0];
            if (direction.x >= max) sprite.sprite = SpriteSet.followers[4];
            if (-direction.x >= max) sprite.sprite = SpriteSet.followers[8];
            if (-direction.y >= max) sprite.sprite = SpriteSet.followers[12];
        }

        public void SetToIcon()
        {
            sprite.sprite = SpriteSet.icons[0];
        }
    }
}