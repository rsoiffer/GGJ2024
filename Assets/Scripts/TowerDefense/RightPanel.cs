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
        public ShadowText ability3;

        public SpriteRenderer item;
        public ShadowText itemName;

        public SpriteRenderer moveType1;
        public ShadowText move1;
        public SpriteRenderer moveType2;
        public ShadowText move2;

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
            ability3.gameObject.SetActive(s.data.HiddenAbilities.Length > 1);
            if (s.data.HiddenAbilities.Length > 1) ability3.SetText(s.data.HiddenAbilities![1]);

            moveType1.enabled = s.moves.Length > 0;
            if (s.moves.Length > 0) moveType1.sprite = typeSprites[(int)s.moves[0].Type];
            move1.gameObject.SetActive(s.moves.Length > 0);
            if (s.moves.Length > 0) move1.SetText(s.moves[0].Name);

            moveType2.enabled = s.moves.Length > 1;
            if (s.moves.Length > 1) moveType2.sprite = typeSprites[(int)s.moves[1].Type];
            move2.gameObject.SetActive(s.moves.Length > 1);
            if (s.moves.Length > 1) move2.SetText(s.moves[1].Name);

            for (var i = 0; i < 6; i++) stats[i].SetText($"{s.GetStat((Stat)i)}");
        }
    }
}