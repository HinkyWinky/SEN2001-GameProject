using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class GameManagerCanvas : CanvasX
    {
        public LoadingPanel loadingPanel;

        public override void Awake()
        {
            base.Awake();
        }

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
