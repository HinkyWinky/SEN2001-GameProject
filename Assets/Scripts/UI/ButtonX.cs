using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [AddComponentMenu("UI/ButtonX")]
    public class ButtonX : Button, IElementUI
    {
        public readonly UnityEvent onDown = new UnityEvent();
        public readonly UnityEvent onUp = new UnityEvent();

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            ButtonDown();
            onDown.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            ButtonUp();
            onUp.Invoke();
        }

        private void ButtonDown()
        {
            GameManager.Cur.Canvas.ActivateUIInput(false);
        }

        private void ButtonUp()
        {
            GameManager.Cur.Canvas.ActivateUIInput(true);
        }

        public void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        public void AddListeners(UnityAction downCall, UnityAction upCall)
        {
            onDown.AddListener(downCall);
            onUp.AddListener(upCall);
        }
        public void RemoveAllListeners()
        {
            onDown.RemoveAllListeners();
            onUp.RemoveAllListeners();
        }
    }
}
