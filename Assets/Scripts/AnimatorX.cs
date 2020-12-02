using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorX : MonoBehaviour
{
    private Animator anim;

    private IEnumerator animationCor;

    [ShowInInspector, ReadOnly] private bool isAnimPlaying;
    public bool IsAnimPlaying => isAnimPlaying;

    [ShowInInspector, ReadOnly] private string currentAnim = "Idle";
    public string CurrentAnim => currentAnim;
    [ShowInInspector, ReadOnly] private string lastAnim = "Idle";

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public bool IsPlaying(string motionName)
    {
        return currentAnim == motionName;
    }
    #region Animation Methods
    public void StartAnimation(string motionName, float duration, bool loop, float fadeDuration)
    {
        StopAnimation();
        animationCor = PlayAnimation(motionName, duration, loop, fadeDuration);
        StartCoroutine(animationCor); // StartLeaf the new animation.
    }
    private IEnumerator PlayAnimation(string motionName, float animationDuration, bool loop, float fadeDuration)
    {
        float fixedAnimationDuration = animationDuration;
        float percent = 0f;

        if (isAnimPlaying && fadeDuration > 0f)
        {
            fixedAnimationDuration = animationDuration - (animationDuration * fadeDuration);
            while (percent <= 1f)
            {
                percent += Time.deltaTime / fadeDuration;
                anim.CrossFade(motionName, fadeDuration);
                yield return null;
            }
        }
        else
        {
            anim.Play(motionName, 0, 0);
        }

        currentAnim = motionName;
        isAnimPlaying = true;
        anim.SetFloat(lastAnim, 0f);
        lastAnim = motionName;

        percent = 0f;
        while (percent <= 1f)
        {
            percent += Time.deltaTime / fixedAnimationDuration;
            anim.SetFloat(motionName, percent);

            // if the loop is true, keep running the while loop
            if (loop && percent >= 1f)
                percent = 0f;

            yield return null;
        }

        // animation is finished
        currentAnim = "";
        isAnimPlaying = false;
    }

    public void StopAnimation()
    {
        if (animationCor != null)
        {
            currentAnim = "";
            StopCoroutine(animationCor); // Stop the last animation.
        }
    }
    #endregion
}
