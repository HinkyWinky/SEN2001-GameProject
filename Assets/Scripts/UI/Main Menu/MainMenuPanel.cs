using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    public class MainMenuPanel : Panel
    {
        [Header("Main Menu Panel Buttons")]
        public ButtonX newGameOrContinueButton;
        public ButtonX levelsButton;
        public ButtonX optionsButton;
        public ButtonX exitButton;

        #region Mono
        private void Start()
        {
            newGameOrContinueButton.AddListeners(NewGameOrContinueButtonOnDown, NewGameOrContinueButtonOnUp);
            levelsButton.AddListeners(LevelsButtonOnDown, LevelsButtonOnUp);
            optionsButton.AddListeners(OptionsButtonOnDown, OptionsButtonOnUp);
            exitButton.AddListeners(ExitButtonOnDown, ExitButtonOnUp);

            SetNewGameOrContinueButton();
        }
        private void OnDestroy()
        {
            newGameOrContinueButton.RemoveAllListeners();
            levelsButton.RemoveAllListeners();
            optionsButton.RemoveAllListeners();
            exitButton.RemoveAllListeners();
        }
        #endregion

        public override void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        #region New Game Or Continue Button
        private void SetNewGameOrContinueButton()
        {
            if (GameManager.Cur.Database.HasSavedLevelFile)
                newGameOrContinueButton.textMesh.SetText("Continue");
            else
                newGameOrContinueButton.textMesh.SetText("New Game");
        }

        private void NewGameOrContinueButtonOnDown()
        {

        }
        private void NewGameOrContinueButtonOnUp()
        {
            int lastCompletedLevel = GameManager.Cur.Database.LastCompletedLevel;
            if (lastCompletedLevel < GameManager.Cur.Database.LastLevel)
                lastCompletedLevel += 1;
            StartCoroutine(GameManager.Cur.SceneCtrl.LoadLevelScene(lastCompletedLevel, true));
        }
        #endregion

        #region LevelsButton
        private void LevelsButtonOnDown()
        {

        }
        private void LevelsButtonOnUp()
        {
            StartCoroutine(LevelsButtonOnUpCor());
        }
        private IEnumerator LevelsButtonOnUpCor()
        {
            GameManager.Cur.MainMenuCanvas.levelsPanel.Activate(true);
            yield return GameManager.Cur.MainMenuCanvas.levelsPanel.StartOpenAnimation(true);
            Activate(false);
        }
        #endregion

        #region OptionsButton
        private void OptionsButtonOnDown()
        {

        }
        private void OptionsButtonOnUp()
        {
            StartCoroutine(OptionsButtonOnUpCor());
        }
        private IEnumerator OptionsButtonOnUpCor()
        {
            GameManager.Cur.MainMenuCanvas.optionsPanel.Activate(true);
            yield return GameManager.Cur.MainMenuCanvas.optionsPanel.StartOpenAnimation(true);
            Activate(false);
        }
        #endregion

        #region ExitButton
        private void ExitButtonOnDown()
        {

        }
        private void ExitButtonOnUp()
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
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

