using System.Collections;

namespace Game.UI
{
    public class InGamePanel : Panel
    {
        public Bar playerHealthBar;
        public Bar enemyHealthBar;

        #region Mono
        #endregion

        public override void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

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

