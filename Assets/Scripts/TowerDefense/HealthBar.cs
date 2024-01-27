using Data;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class HealthBar : MonoBehaviour
    {
        public PokemonInstance pokemon;
        public Image fill;
        public GameObject canvas;

        public Color green;
        public Color yellow;
        public Color red;

        private void Update()
        {
            canvas.SetActive(!pokemon.inBox);
            var currentHealthPerc = (float)(pokemon.GetStat(Stat.HP) - pokemon.damageTaken) / pokemon.GetStat(Stat.HP);
            fill.rectTransform.sizeDelta = new Vector2(currentHealthPerc * .75f, 0);
            fill.color = currentHealthPerc switch
            {
                > .5f => green,
                > .2f => yellow,
                _ => red
            };
        }
    }
}