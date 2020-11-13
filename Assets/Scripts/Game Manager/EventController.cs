using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventController : MonoBehaviour
{
    public UnityEvent onMainMenuOpen;
    public UnityEventInt onLevelOpen;

}

[Serializable] public class UnityEventInt : UnityEvent<int> { }
[Serializable] public class UnityEventFloat : UnityEvent<float> { }
