using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class SceneCanvas : CanvasX
    {
        public PausePanel pausePanel;
        public InGamePanel inGamePanel;

        public override void Activate(bool value)
        {
            base.Activate(value);
            gameObject.SetActive(value);
        }
    }
}
