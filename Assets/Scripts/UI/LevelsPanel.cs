using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class LevelsPanel : MonoBehaviour, IElementUI
    {
        public void Activate(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}

