using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class OptionsPanel : MonoBehaviour, IElementUI
    {
        [Header("Option Buttons")]
        public ButtonX backButton;

        [Header("Panels")]
        [SerializeField] private MainMenuPanel mainMenuPanel = default;

        private void Start()
        {
            backButton.AddListeners(BackButtonOnDown, BackButtonOnUp);
        }

        private void OnDestroy()
        {
            backButton.RemoveAllListeners();
        }

        public void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        private void BackButtonOnDown()
        {

        }
        private void BackButtonOnUp()
        {
            mainMenuPanel.Activate(true);
            Activate(false);
        }


    }
}

