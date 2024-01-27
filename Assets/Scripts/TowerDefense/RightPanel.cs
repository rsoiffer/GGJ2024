using Data;
using UnityEngine;

namespace TowerDefense
{
    public class RightPanel : MonoBehaviour
    {
        public SelectionController selection;
        public Sprite[] typeSprites;

        public GameObject parent;

        public ShadowText name;
        public ShadowText level;
        public SpriteRenderer type1;
        public SpriteRenderer type2;
        public SpriteRenderer pokemon;
        public ShadowText ability1;
        public ShadowText ability2;

        public SpriteRenderer item;
        public ShadowText itemName;

        public ShadowText[] stats;

        private void Update()
        {
            var s = selection.selected;
            parent.SetActive(s != null);
            if (s == null) return;

            name.SetText(s.data.Name);
            level.SetText($"Lv.{s.level}");
            type1.sprite = typeSprites[(int)s.data.Types[0]];
            type2.enabled = s.data.Types.Length > 1;
            if (s.data.Types.Length > 1) type2.sprite = typeSprites[(int)s.data.Types[1]];
            pokemon.sprite = s.sprite.SpriteSet.front;
            ability1.SetText(s.data.Abilities[0]);
            ability2.SetText(s.data.HiddenAbilities![0]);

            for (var i = 0; i < 6; i++) stats[i].SetText($"{s.GetStat((Stat)i)}");
        }
    }
}