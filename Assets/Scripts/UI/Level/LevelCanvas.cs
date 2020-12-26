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
        public void OpenEndLevelPanelOnPlayerDie()
        {
            endLevelPanel.Activate(true);
            StartCoroutine(endLevelPanel.OpenOnPlayerDieCor());
        }
        public void OpenEndLevelPanelOnEnemyDie()
        {
            endLevelPanel.Activate(true);
            StartCoroutine(endLevelPanel.OpenOnEnemyDieCor());
        }
        #endregion
    }
}
