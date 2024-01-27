using UnityEngine;

namespace TowerDefense
{
    public class SlotSpawner : MonoBehaviour
    {
        public GameObject slotExample;
        public int nx;
        public int ny;
        public Vector2 offset;

        private void Start()
        {
            for (var x = 0; x < nx; x++)
            for (var y = 0; y < ny; y++)
                if (x != 0 || y != 0)
                {
                    var newSlot = Instantiate(slotExample, slotExample.transform.parent);
                    newSlot.transform.position += (Vector3)Vector2.Scale(offset, new Vector2(x, y));
                }
        }
    }
}