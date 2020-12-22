using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class Bar : MonoBehaviour, IElementUI
    {
        private Slider bar;

        private void Awake()
        {
            bar = GetComponent<Slider>();
        }

        public void SetValue(float value)
        {
            bar.value = value;
        }
        public void SetValue(int value, int maxValue)
        {
            bar.value = (float) value / maxValue;
        }

        public void Activate(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
