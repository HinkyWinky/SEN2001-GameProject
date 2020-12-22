using System.Collections;
using UnityEditor;
using UnityEngine;
namespace Game.UI
{
    public class MainMenuPanel : Panel
    {
        [Header("Main Menu Panel Buttons")]
        public ButtonX newGameButton;
        public ButtonX levelsButton;
        public ButtonX optionsButton;
        public ButtonX exitButton;

        #region Mono
        private void Start()
        {
            newGameButton.AddListeners(NewGameButtonOnDown, NewGameButtonOnUp);
            levelsButton.AddListeners(LevelsButtonOnDown, LevelsButtonOnUp);
            optionsButton.AddListeners(OptionsButtonOnDown, OptionsButtonOnUp);
            exitButton.AddListeners(ExitButtonOnDown, ExitButtonOnUp);
        }
        private void OnDestroy()
        {
            newGameButton.RemoveAllListeners();
            levelsButton.RemoveAllListeners();
            optionsButton.RemoveAllListeners();
            exitButton.RemoveAllListeners();
        }
        #endregion

        public override void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        #region New Game / Continue Button
        private void NewGameButtonOnDown()
        {
            Debug.Log("New Game Button pointer down");
        }
        private void NewGameButtonOnUp()
        {
            Debug.Log("New Game Button pointer up");
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

