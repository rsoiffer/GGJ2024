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

        public Transform hpBar;
        public SpriteRenderer hpBarFill;
        public Color green;
        public Color yellow;
        public Color red;
        public Transform expBar;

        public ShadowText ability1;
        public ShadowText ability2;
        public ShadowText ability3;

        public SpriteRenderer item;
        public ShadowText itemName;

        public SpriteRenderer[] moveTypes;
        public ShadowText[] moves;

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

            var currentHealthPerc = Mathf.Clamp01((float)(s.GetStat(Stat.HP) - s.damageTaken) / s.GetStat(Stat.HP));
            hpBar.transform.localScale = new Vector3(currentHealthPerc, 1, 1);
            hpBarFill.color = currentHealthPerc switch
            {
                > .5f => green,
                > .2f => yellow,
                _ => red
            };
            var currentExpPerc = Mathf.Clamp01((s.experience - ExpManager.MinXpByLevel(s, s.level)) /
                                               (ExpManager.MinXpByLevel(s, s.level + 1) -
                                                ExpManager.MinXpByLevel(s, s.level)));
            expBar.transform.localScale = new Vector3(currentExpPerc, 1, 1);

            ability1.SetText(s.data.Abilities[0]);
            ability2.SetText(s.data.HiddenAbilities![0]);
            ability3.gameObject.SetActive(s.data.HiddenAbilities.Length > 1);
            if (s.data.HiddenAbilities.Length > 1) ability3.SetText(s.data.HiddenAbilities![1]);

            for (var i = 0; i < 4; i++)
            {
                moveTypes[i].enabled = s.attacks[i].Active;
                if (s.attacks[i].Active) moveTypes[i].sprite = typeSprites[(int)s.attacks[i].move.Type];
                moves[i].gameObject.SetActive(s.attacks[i].Active);
                if (s.attacks[i].Active) moves[i].SetText(s.attacks[i].move.Name);
            }

            for (var i = 0; i < 6; i++) stats[i].SetText($"{s.GetStat((Stat)i)}");
        }
    }
}