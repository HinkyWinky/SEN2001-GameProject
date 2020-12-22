using System.Collections;
using UnityEngine;

namespace Game.UI
{
    public class OptionsPanel : Panel
    {
        [Header("Options Buttons")]
        public ButtonX backButton;

        [Header("Animation")]
        [SerializeField] private float timeOpenCloseAnimation = 0.5f;

        #region Mono
        private void Start()
        {
            backButton.AddListeners(BackButtonOnDown, BackButtonOnUp);
            Activate(false);
        }
        private void OnDestroy()
        {
            backButton.RemoveAllListeners();
        }
        #endregion

        public override void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        #region Back Button
        private void BackButtonOnDown()
        {

        }
        private void BackButtonOnUp()
        {
            StartCoroutine(BackButtonOnUpCor());
        }
        private IEnumerator BackButtonOnUpCor()
        {
            GameManager.Cur.MainMenuCanvas.mainMenuPanel.Activate(true);
            yield return StartCoroutine(StartCloseAnimation(true));
            Activate(false);
        }
        #endregion

        #region Animations
        public override IEnumerator OpenAnimation()
        {
            GameManager.Cur.Canvas.ActivateUIInput(false);
            yield return StartCoroutine(ScalePanel(Vector3.zero, rectTransform.localScale, timeOpenCloseAnimation));
            GameManager.Cur.Canvas.ActivateUIInput(true);
        }
        public override IEnumerator CloseAnimation()
        {
            GameManager.Cur.Canvas.ActivateUIInput(false);
            yield return StartCoroutine(ScalePanel(rectTransform.localScale, Vector3.zero, timeOpenCloseAnimation));
            GameManager.Cur.Canvas.ActivateUIInput(true);
        }
        #endregion
    }
}

