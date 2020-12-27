using UnityEngine;

namespace Game.UI
{
    public abstract class UIElement : MonoBehaviour, IElementUI
    {
        [HideInInspector] public RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Activate(bool value)
        {
            throw new System.NotImplementedException();
        }
    }
}
