using JetBrains.Annotations;
using UnityEngine;

namespace TowerDefense
{
    public class SelectionController : MonoBehaviour
    {
        public static SelectionController Instance;

        public AudioSource cryAudioSource;
        public float slotSelectionRadius = .5f;

        [CanBeNull] public FriendlyAI dragging;
        [CanBeNull] public Slot draggingItem;
        public PokemonInstance selected;

        private void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            Vector2 mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                SetSelected(PokemonInstance.GetNearest(mousePos, .5f, _ => true));
                dragging = selected == null ? null : selected.GetComponent<FriendlyAI>();

                if (selected == null)
                    draggingItem = Slot.GetSlot(mousePos, slotSelectionRadius, s => s.ItemInSlot != null);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (dragging != null)
                {
                    var oldSlot = Slot.GetSlot(dragging);
                    var newSlot = Slot.GetSlot(mousePos, slotSelectionRadius,
                        s => !s.AnyInSlot || s.InSlot == dragging);

                    if (newSlot != null && newSlot != oldSlot && !newSlot.AnyInSlot)
                    {
                        if (oldSlot != null)
                            oldSlot.Set((FriendlyAI)null);
                        dragging.pokemon.MoveToSlot(newSlot);
                        newSlot.Set(dragging);

                        if (newSlot.isTrash) Destroy(dragging.gameObject);
                    }
                }
                else if (draggingItem != null)
                {
                    var newSlot = Slot.GetSlot(mousePos, slotSelectionRadius, s => s.InSlot != null || s.acceptsItems);
                    if (newSlot != null)
                    {
                        if (newSlot.InSlot != null)
                        {
                            var pokeItem = newSlot.InSlot.pokemon.item;
                            newSlot.InSlot.pokemon.item = draggingItem.ItemInSlot;
                            draggingItem.Set(pokeItem);
                        }
                        else if (newSlot != draggingItem)
                        {
                            var otherItem = newSlot.ItemInSlot;
                            newSlot.Set(draggingItem.ItemInSlot);
                            draggingItem.Set(otherItem);
                        }
                    }
                }

                dragging = null;
            }
        }

        private void SetSelected([CanBeNull] PokemonInstance newSelected)
        {
            if (selected == newSelected) return;

            if (selected != null) selected.sprite.selectionRing.enabled = false;
            if (newSelected != null) newSelected.sprite.selectionRing.enabled = true;

            selected = newSelected;

            if (selected != null)
            {
                cryAudioSource.clip = selected.data.cry;
                cryAudioSource.Play();
            }
        }
    }
}