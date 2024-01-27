using UnityEngine;

namespace TowerDefense
{
    public class FriendlyAI : MonoBehaviour
    {
        public PokemonInstance pokemon;
        public string id;

        private void Start()
        {
            if (string.IsNullOrEmpty(pokemon.data.Id))
            {
                pokemon.ResetTo(id, pokemon.level);
                pokemon.Move(pokemon.transform.position);
            }

            if (Slot.GetSlot(this) == null) Slot.GetSlot(transform.position, 100, _ => true)!.Set(this);
        }
    }
}