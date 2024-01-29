using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class SpeedController : MonoBehaviour
    {
        public Toggle toggle;
        public float normalSpeed = 1;
        public float fastSpeed = 4;

        private void Update()
        {
            Time.timeScale = toggle.isOn ? fastSpeed : normalSpeed;
        }
    }
}