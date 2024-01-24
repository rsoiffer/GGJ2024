using UnityEngine;

namespace TowerDefense
{
    public class GridManager : MonoBehaviour
    {
        public float cellSize = .5f;

        public Vector2Int ToCell(Vector2 pos)
        {
            return new Vector2Int(Mathf.RoundToInt(pos.x / cellSize), Mathf.RoundToInt(pos.y / cellSize));
        }

        public Vector2 FromCell(Vector2Int cell)
        {
            return (Vector2)cell * cellSize;
        }
    }
}