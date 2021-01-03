namespace Game.UI
{
    public class LevelCanvas : CanvasX
    {
        public InGamePanel inGamePanel;
        public PausePanel pausePanel;
        public EndLevelPanel endLevelPanel;

        public override void Activate(bool value)
        {
            base.Activate(value);
            gameObject.SetActive(value);
        }

        #region End Level Panel
        public void OpenEndLevelPanel(LevelResults result)
        {
            endLevelPanel.Activate(true);
            switch (result)
            {
                case LevelResults.DEFEAT:
                    StartCoroutine(endLevelPanel.OpenDefeatPanel());
                    break;
                case LevelResults.VICTORY:
                    StartCoroutine(endLevelPanel.OpenVictoryPanel());
                    break;
            }
        }
        #endregion
    }
}
