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
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (dragging != null)
                {
                    var oldSlot = Slot.GetSlot(dragging);
                    var newSlot = Slot.GetSlot(mousePos, slotSelectionRadius,
                        s => s.InSlot == null || s.InSlot == dragging);

                    if (newSlot != null && newSlot != oldSlot && newSlot.InSlot == null)
                    {
                        if (oldSlot != null)
                            oldSlot.Set(null);
                        dragging.pokemon.MoveToSlot(newSlot);
                        newSlot.Set(dragging);

                        if (newSlot.isTrash) Destroy(dragging.gameObject);
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

            if (selected != null)
            {
                cryAudioSource.clip = selected.data.cry;
                cryAudioSource.Play();
            }
        }
    }
}