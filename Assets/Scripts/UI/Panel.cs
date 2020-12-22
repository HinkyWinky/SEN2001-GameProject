using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI
{
    [Serializable]
    public abstract class Panel : MonoBehaviour, IElementUI
    {
        [ReadOnly] public RectTransform rectTransform;
        [ReadOnly] public CanvasGroup canvasGroup;
        [ReadOnly] public Vector3 originalScale;

        private void Reset()
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        public virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalScale = rectTransform.localScale;
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public abstract void Activate(bool value);

        public IEnumerator StartOpenAnimation(bool waitAnimation)
        {
            if (waitAnimation)
                yield return StartCoroutine(OpenAnimation());
            else
            {
                StartCoroutine(OpenAnimation());
                yield return null;
            }
        }
        public abstract IEnumerator OpenAnimation();

        public IEnumerator StartCloseAnimation(bool waitAnimation)
        {
            if (waitAnimation)
                yield return StartCoroutine(CloseAnimation());
            else
            {
                StartCoroutine(CloseAnimation());
                yield return null;
            }
        }
        public abstract IEnumerator CloseAnimation();

        public IEnumerator ScalePanel(Vector3 initialScale, Vector3 targetScale, float time)
        {
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.unscaledDeltaTime / time;
                float func = Mathf.Pow(percent, 2);

                rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, func);

                yield return null;
            }

            rectTransform.localScale = originalScale;
        }
    }
}
