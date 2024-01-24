using System.Linq;
using Data;
using UnityEngine;

namespace TowerDefense
{
    public class PokemonInstance : MonoBehaviour
    {
        public PokemonDatabase database;
        public SpriteRenderer sprite;

        private PokemonData data;
        private int level;

        public void ResetTo(string id, int level)
        {
            data = database.database.First(p => p.Id == id);
            this.level = level;
        }

        public void Move(Vector2 pos)
        {
            var direction = pos - (Vector2)transform.position;
            transform.position = pos;
            if (direction.magnitude < 1e-3f) return;
            var max = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
            if (direction.y >= max) sprite.sprite = data.spriteSet.followers[0];
            if (direction.x >= max) sprite.sprite = data.spriteSet.followers[4];
            if (-direction.x >= max) sprite.sprite = data.spriteSet.followers[8];
            if (-direction.y >= max) sprite.sprite = data.spriteSet.followers[12];
        }
    }
}