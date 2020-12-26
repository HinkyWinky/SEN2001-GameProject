using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class EndLevelPanel : Panel
    {
        public TextMeshProUGUI levelResultText;
        public Image buttonGroup;

        [Header("End Level Panel Buttons")]
        public ButtonX nextLevelOrRestartButton;
        public ButtonX returnToMainMenuButton;

        [Header("Animation")]
        [SerializeField] private float timeOpenCloseAnimation = 0.5f;

        #region Mono
        private void Start()
        {
            nextLevelOrRestartButton.AddListeners(NextLevelOrRestartButtonOnDown, NextLevelOrRestartButtonOnUp);
            returnToMainMenuButton.AddListeners(ReturnToMainMenuButtonOnDown, ReturnToMainMenuButtonOnUp);

            Activate(false);
        }
        private void OnDestroy()
        {
            nextLevelOrRestartButton.RemoveAllListeners();
            returnToMainMenuButton.RemoveAllListeners();
        }
        #endregion

        public override void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        public IEnumerator OpenOnPlayerDieCor()
        {
            if (nextLevelOrRestartButton.HasText)
                nextLevelOrRestartButton.textMesh.SetText("Restart");
            levelResultText.SetText("DEFEAT");
            GameManager.Cur.EventCtrl.onEndLevelPanelOpened?.Invoke(LevelResults.DEFEAT);
            yield return StartCoroutine(StartOpenAnimation(true));
        }
        public IEnumerator OpenOnEnemyDieCor()
        {
            if (nextLevelOrRestartButton.HasText)
            {
                if (GameManager.Cur.SceneCtrl.CurrentLevelNo == GameManager.Cur.SceneCtrl.MaxLevelNo)
                {
                    nextLevelOrRestartButton.textMesh.SetText("All Levels Done");
                    nextLevelOrRestartButton.interactable = false;
                }
                else
                {
                    nextLevelOrRestartButton.textMesh.SetText("Next Level");
                }
            }

            levelResultText.SetText("VICTORY");
            GameManager.Cur.EventCtrl.onEndLevelPanelOpened?.Invoke(LevelResults.VICTORY);
            yield return StartCoroutine(StartOpenAnimation(true));
        }

        #region Next Level or Restart Button
        private void NextLevelOrRestartButtonOnDown()
        {

        }
        private void NextLevelOrRestartButtonOnUp()
        {
            if (nextLevelOrRestartButton.textMesh.text.Equals("Restart"))
            {
                StartCoroutine(GameManager.Cur.SceneCtrl.LoadLevelScene(
                    GameManager.Cur.SceneCtrl.CurrentLevelNo, true));
            }
            else if (nextLevelOrRestartButton.textMesh.text.Equals("Next Level"))
            {
                StartCoroutine(GameManager.Cur.SceneCtrl.LoadLevelScene(
                    GameManager.Cur.SceneCtrl.CurrentLevelNo + 1, true));
            }
            else
                Debug.LogWarning("Check strings!");
        }
        #endregion

        #region Return to Main Menu Button
        private void ReturnToMainMenuButtonOnDown()
        {

        }
        private void ReturnToMainMenuButtonOnUp()
        {
            StartCoroutine(GameManager.Cur.SceneCtrl.LoadMainMenuScene(true));
        }
        #endregion

        #region Animations
        public override IEnumerator OpenAnimation()
        {
            GameManager.Cur.Canvas.ActivateUIInput(false);
            yield return StartCoroutine(ScalePanel(
                Vector3.zero, rectTransform.localScale, timeOpenCloseAnimation));
            GameManager.Cur.Canvas.ActivateUIInput(true);
        }
        public override IEnumerator CloseAnimation()
        {
            GameManager.Cur.Canvas.ActivateUIInput(false);
            yield return StartCoroutine(ScalePanel(
                rectTransform.localScale, Vector3.zero, timeOpenCloseAnimation));
            GameManager.Cur.Canvas.ActivateUIInput(true);
        }
        #endregion
    }
}
