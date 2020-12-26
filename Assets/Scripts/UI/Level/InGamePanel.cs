using System.Collections;

namespace Game.UI
{
    public class InGamePanel : Panel
    {
        public Bar playerHealthBar;
        public Bar enemyHealthBar;
        public ButtonX pauseButton;

        #region Mono
        private void Start()
        {
            pauseButton.AddListeners(PauseButtonOnDown, PauseButtonOnUp);
        }
        private void OnDestroy()
        {
            pauseButton.RemoveAllListeners();
        }
        #endregion

        public override void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        #region Pause Button
        private void PauseButtonOnDown()
        {

        }
        private void PauseButtonOnUp()
        {
            StartCoroutine(PauseButtonOnUpCor());
        }
        private IEnumerator PauseButtonOnUpCor()
        {
            GameManager.Cur.LevelCanvas.pausePanel.Activate(true);
            GameManager.Cur.EventCtrl.onPausePanelOpened?.Invoke();
            yield return GameManager.Cur.LevelCanvas.pausePanel.StartOpenAnimation(true);
        }
        #endregion

        #region Animations
        public override IEnumerator OpenAnimation()
        {
            throw new System.NotImplementedException();
        }
        public override IEnumerator CloseAnimation()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}

