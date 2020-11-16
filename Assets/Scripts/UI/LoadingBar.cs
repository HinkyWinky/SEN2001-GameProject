using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LoadingBar : MonoBehaviour, IElementUI
    {
        public Slider bar;

        public void Activate(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
