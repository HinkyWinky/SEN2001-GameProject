using UnityEngine;

namespace UI
{
    public class MainMenuPanel : MonoBehaviour, IElementUI
    {
        [Header("Main Menu Buttons")]
        public CanvasGroup mainMenuButtonsPanel;
        public ButtonX newGameButton;
        public ButtonX levelsButton;
        public ButtonX optionsButton;
        public ButtonX exitButton;

        [Header("Panels")]
        [SerializeField] private OptionsPanel optionsPanel = default;
        [SerializeField] private LevelsPanel levelsPanel = default;

        private void Start()
        {
            newGameButton.AddListeners(NewGameButtonOnDown, NewGameButtonOnUp);
            levelsButton.AddListeners(LevelsButtonOnDown, LevelsButtonOnUp);
            optionsButton.AddListeners(OptionsButtonOnDown, OptionsButtonOnUp);
            exitButton.AddListeners(ExitButtonOnDown, ExitButtonOnUp);

            levelsPanel.Activate(false);
            optionsPanel.Activate(false);
        }

        private void OnDestroy()
        {
            newGameButton.RemoveAllListeners();
            levelsButton.RemoveAllListeners();
            optionsButton.RemoveAllListeners();
            exitButton.RemoveAllListeners();
        }

        private void NewGameButtonOnDown()
        {
            Debug.Log("New Game Button pointer down");
        }
        private void NewGameButtonOnUp()
        {
            Debug.Log("New Game Button pointer up");
        }

        private void LevelsButtonOnDown()
        {

        }
        private void LevelsButtonOnUp()
        {

        }

        private void OptionsButtonOnDown()
        {

        }
        private void OptionsButtonOnUp()
        {
            optionsPanel.Activate(true);
            Activate(false);
        }

        private void ExitButtonOnDown()
        {

        }
        private void ExitButtonOnUp()
        {

        }

        public void Activate(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}

