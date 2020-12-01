using System;
using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private AnimatorX animX;
    private Animator anim;

    public int health = 3;
    public int maxHealth = 3;

    private void Awake()
    {
        animX = GetComponent<AnimatorX>();
        anim = GetComponent<Animator>();
    }
}
