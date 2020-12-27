namespace Game.UI
{
    public class GameManagerCanvas : CanvasX
    {
        public LoadingPanel loadingPanel;

        private void Start()
        {
            Activate(false);
        }

        public override void Activate(bool value)
        {
            base.Activate(value);
            gameObject.SetActive(value);
        }
    }
}
