using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private AnimatorX animX;

    private void Awake()
    {
        animX = GetComponent<AnimatorX>();
    }

    private void Start()
    {
        StartCoroutine(AnimPlay());
    }
    IEnumerator AnimPlay()
    {
        yield return new WaitForSeconds(3f);
        animX.StopAnimation();
        animX.StartAnimation("Move Right", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Up", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Right", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Up", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Right", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Up", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Right", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Up", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Right", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Up", 10f, false, 0.6f);
        yield return new WaitForSeconds(7f);
        animX.StopAnimation();
        animX.StartAnimation("Move Right", 10f, false, 0.6f);
    }
}
