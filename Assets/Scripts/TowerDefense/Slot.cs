using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Helpers;
using JetBrains.Annotations;
using UnityEngine;

namespace TowerDefense
{
    public class Slot : MonoBehaviour
    {
        private static readonly HashSet<Slot> AllSlots = new();

        public bool isBox;
        public bool isTrash;
        public bool acceptsItems = true;
        public SpriteRenderer itemSprite;

        [CanBeNull] public FriendlyAI InSlot { get; private set; }
        [CanBeNull] public ItemData ItemInSlot { get; private set; }

        public bool AnyInSlot => InSlot != null || (ItemInSlot != null && !string.IsNullOrEmpty(ItemInSlot.Id));

        private void LateUpdate()
        {
            if (itemSprite != null)
            {
                itemSprite.enabled = ItemInSlot != null;
                if (ItemInSlot != null) itemSprite.sprite = ItemInSlot.Sprite;
            }
        }

        private void OnEnable()
        {
            AllSlots.Add(this);
        }

        private void OnDisable()
        {
            AllSlots.Remove(this);
        }

        public void Set(FriendlyAI pokemon)
        {
            if (pokemon != null && GetSlot(pokemon) != null) Debug.LogError("Oh no");
            InSlot = pokemon;
        }

        public void Set(ItemData item)
        {
            ItemInSlot = item;
        }

        [CanBeNull]
        public static Slot GetSlot(FriendlyAI pokemon)
        {
            return AllSlots.SingleOrDefault(s => s.InSlot != null && s.InSlot == pokemon);
        }

        [CanBeNull]
        public static Slot GetSlot(Vector2 position, float maxRange, Func<Slot, bool> filter)
        {
            var nearestOther = AllSlots.Where(filter)
                .Where(p => Vector2.Distance(position, p.transform.position) < maxRange)
                .MinByOrElse(
                    p => Vector2.Distance(position, p.transform.position) + (p.isBox || p.InSlot != null ? 0 : 100),
                    null);
            return nearestOther;
        }
    }
}