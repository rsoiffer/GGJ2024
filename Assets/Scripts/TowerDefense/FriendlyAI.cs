using UnityEngine;

namespace TowerDefense
{
    public class FriendlyAI : MonoBehaviour
    {
        public PokemonInstance pokemon;
        public string id;

        private void Start()
        {
            pokemon.ResetTo(id, pokemon.level);
            pokemon.Move(pokemon.transform.position);
        }
    }
}