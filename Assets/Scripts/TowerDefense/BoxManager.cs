using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using JetBrains.Annotations;
using UnityEngine;

namespace TowerDefense
{
    public class BoxManager : MonoBehaviour
    {
        public GameObject slotExample;
        public int nx;
        public int ny;
        public Vector2 offset;
        public float selectionRadius = .5f;

        [ItemCanBeNull] public Dictionary<GameObject, PokemonInstance> Slots;

        private void Start()
        {
            Slots = new Dictionary<GameObject, PokemonInstance> { { slotExample, null } };
            for (var x = 0; x < nx; x++)
            for (var y = 0; y < ny; y++)
                if (x != 0 || y != 0)
                {
                    var newSlot = Instantiate(slotExample, slotExample.transform.parent);
                    newSlot.transform.position += (Vector3)Vector2.Scale(offset, new Vector2(x, y));
                    Slots.Add(newSlot, null);
                }
        }

        [CanBeNull]
        public GameObject GetSlot(Vector2 position, Func<GameObject, bool> filter)
        {
            var nearestOther = Slots.Keys.Where(filter)
                .MinByOrElse(p => Vector2.Distance(position, p.transform.position), null);
            if (nearestOther == null) return null;
            var dist = Vector2.Distance(position, nearestOther.transform.position);
            return dist < selectionRadius ? nearestOther : null;
        }
    }
}