using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class GridManager : MonoBehaviour
    {
        public float cellSize = .5f;
        public Vector2 offset = new(.25f, .25f);

        public Dictionary<Vector2Int, PokemonInstance> pokemon = new();

        public Vector2Int ToCell(Vector2 pos)
        {
            var pos2 = (pos - offset) / cellSize;
            return new Vector2Int(Mathf.RoundToInt(pos2.x), Mathf.RoundToInt(pos2.y));
        }

        public Vector2 FromCell(Vector2Int cell)
        {
            return (Vector2)cell * cellSize + offset;
        }
    }
}