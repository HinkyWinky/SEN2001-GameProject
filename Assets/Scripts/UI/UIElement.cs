using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;

public abstract class UIElement : MonoBehaviour, IElementUI
{
    [HideInInspector] public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Activate(bool value)
    {
        throw new System.NotImplementedException();
    }
}
