using UnityEngine;

namespace TowerDefense
{
    public class SpriteController : MonoBehaviour
    {
        public PokemonInstance pokemon;
        public SpriteRenderer sprite;
        public SpriteRenderer selectionRing;

        public void Look(Vector2 direction)
        {
            var max = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
            if (direction.y >= max) sprite.sprite = pokemon.data.spriteSet.followers[0];
            if (direction.x >= max) sprite.sprite = pokemon.data.spriteSet.followers[4];
            if (-direction.x >= max) sprite.sprite = pokemon.data.spriteSet.followers[8];
            if (-direction.y >= max) sprite.sprite = pokemon.data.spriteSet.followers[12];
        }

        public void SetToIcon()
        {
            sprite.sprite = pokemon.data.spriteSet.icons[0];
        }
    }
}