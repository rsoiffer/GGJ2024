using JetBrains.Annotations;
using UnityEngine;

namespace TowerDefense
{
    public class SelectionController : MonoBehaviour
    {
        public GridManager gridManager;
        public BoxManager boxManager;

        public PokemonInstance dragging;
        public PokemonInstance selected;

        public void Update()
        {
            Vector2 mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                SetSelected(PokemonInstance.GetNearest(mousePos, .5f, _ => true));
                dragging = selected;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (dragging != null && dragging.isFriendly)
                {
                    var oldCell = gridManager.ToCell(dragging.transform.position);
                    var newCell = gridManager.ToCell(mousePos);

                    var slot = boxManager.GetSlot(mousePos, _ => true);
                    if (slot != null)
                    {
                        if (boxManager.Slots[slot] == null)
                        {
                            gridManager.pokemon.Remove(oldCell);
                            dragging.MoveToBox(slot);
                            boxManager.Slots[slot] = dragging;
                        }
                    }
                    else if (!gridManager.pokemon.ContainsKey(newCell))
                    {
                        gridManager.pokemon.Remove(oldCell);
                        dragging.Move(gridManager.FromCell(newCell));
                        gridManager.pokemon.Add(newCell, dragging);
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