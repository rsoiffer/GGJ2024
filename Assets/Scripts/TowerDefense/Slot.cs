using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using JetBrains.Annotations;
using UnityEngine;

namespace TowerDefense
{
    public class Slot : MonoBehaviour
    {
        private static readonly HashSet<Slot> AllSlots = new();

        [CanBeNull] public FriendlyAI inSlot;
        public bool isBox;

        private void OnEnable()
        {
            AllSlots.Add(this);
        }

        private void OnDisable()
        {
            AllSlots.Remove(this);
        }

        [CanBeNull]
        public static Slot GetSlot(FriendlyAI pokemon)
        {
            return AllSlots.SingleOrDefault(s => s.inSlot != null && s.inSlot == pokemon);
        }

        [CanBeNull]
        public static Slot GetSlot(Vector2 position, float maxRange, Func<Slot, bool> filter)
        {
            var nearestOther = AllSlots.Where(filter)
                .Where(p => Vector2.Distance(position, p.transform.position) < maxRange)
                .MinByOrElse(
                    p => Vector2.Distance(position, p.transform.position) + (p.isBox || p.inSlot != null ? 0 : 100),
                    null);
            return nearestOther;
        }
    }
}