using JetBrains.Annotations;
using UnityEngine;

namespace TowerDefense
{
    public class SelectionController : MonoBehaviour
    {
        public float slotSelectionRadius = .5f;

        [CanBeNull] public FriendlyAI dragging;
        public PokemonInstance selected;

        public void Update()
        {
            Vector2 mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                SetSelected(PokemonInstance.GetNearest(mousePos, .5f, _ => true));
                dragging = selected == null ? null : selected.GetComponent<FriendlyAI>();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (dragging != null)
                {
                    var oldSlot = Slot.GetSlot(dragging);
                    var newSlot = Slot.GetSlot(mousePos, slotSelectionRadius, _ => true);

                    if (newSlot != null && newSlot != oldSlot && newSlot.InSlot == null)
                    {
                        if (oldSlot != null)
                            oldSlot.Set(null);
                        dragging.pokemon.MoveToSlot(newSlot);
                        newSlot.Set(dragging);
                    }
                }

                dragging = null;
            }
        }

        public void SetSelected([CanBeNull] PokemonInstance newSelected)
        {
            if (selected == newSelected) return;

            if (selected != null) selected.sprite.selectionRing.enabled = false;
            if (newSelected != null) newSelected.sprite.selectionRing.enabled = true;

            selected = newSelected;
        }
    }
}