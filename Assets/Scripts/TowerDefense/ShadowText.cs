using TMPro;
using UnityEngine;

namespace TowerDefense
{
    public class ShadowText : MonoBehaviour
    {
        public TextMeshProUGUI[] texts;

        private void Awake()
        {
            texts = GetComponentsInChildren<TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            foreach (var t in texts) t.text = text;
        }

        public void SetColors(Color main, Color shadow)
        {
            for (var i = 0; i < texts.Length; i++) texts[i].color = i < texts.Length - 1 ? main : shadow;
        }
    }
}