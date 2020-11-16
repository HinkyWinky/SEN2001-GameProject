using UnityEngine;

namespace Game.UI
{
    public class MainMenuCanvas : CanvasX
    {
        public MainMenuPanel mainMenuPanel;
        public LevelsPanel levelsPanel;
        public OptionsPanel optionsPanel;

        public override void Awake()
        {
            base.Awake();
        }

        public override void Activate(bool value)
        {
            base.Activate(value);
            gameObject.SetActive(value);
        }
    }
}
