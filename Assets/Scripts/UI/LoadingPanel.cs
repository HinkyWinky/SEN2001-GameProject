using System.Collections;

namespace Game.UI
{
    public class LoadingPanel : Panel
    {
        public Bar bar;

        public override void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        public override IEnumerator OpenAnimation()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerator CloseAnimation()
        {
            throw new System.NotImplementedException();
        }
    }
}
