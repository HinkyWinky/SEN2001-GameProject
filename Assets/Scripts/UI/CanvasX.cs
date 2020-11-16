using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public abstract class CanvasX : MonoBehaviour, IElementUI
    {
        private GraphicRaycaster graphicRaycaster;

        public virtual void Awake()
        {
            graphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        public void ActivateUIInput(bool value)
        {
            graphicRaycaster.enabled = value;
        }

        public virtual void Activate(bool value)
        {
            
        }
    }
}
