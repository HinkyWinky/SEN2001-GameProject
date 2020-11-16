using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;

namespace Game.UI
{
    public class PausePanel : Panel
    {
        public ButtonX returnMainMenuButton;

        #region Mono
        public override void Awake()
        {
            base.Awake();
        }
        private void Start()
        {
            returnMainMenuButton.AddListeners(ReturnMainMenuButtonOnDown, ReturnMainMenuButtonOnUp);
        }
        private void OnDestroy()
        {
            returnMainMenuButton.RemoveAllListeners();
        }
        #endregion

        public override void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        #region Return Main Menu Button
        private void ReturnMainMenuButtonOnDown()
        {

        }
        private void ReturnMainMenuButtonOnUp()
        {
            StartCoroutine(GameManager.Cur.SceneController.LoadMainMenuScene(true));
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

