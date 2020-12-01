using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorX : MonoBehaviour
{
    private Animator anim;

    private IEnumerator animationCor;
    private WaitForFixedUpdate waitForFixedUpdate;

    private bool isPlaying;
    public bool IsPlaying => isPlaying;

    private string lastAnim = "Idle";
    private string currentAnim = "Idle";
    public string CurrentAnim => currentAnim;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    #region Animation Methods
    public void StartAnimation(string motionName, float duration, bool loop, float fadeDuration)
    {
        StopAnimation();
        animationCor = PlayAnimation(motionName, duration, loop, fadeDuration);
        StartCoroutine(animationCor); // Start the new animation.
    }
    private IEnumerator PlayAnimation(string motionName, float animationDuration, bool loop, float fadeDuration)
    {
        currentAnim = motionName;
        isPlaying = true;
        float fixedAnimationDuration = animationDuration - (animationDuration * fadeDuration);

        float percent = 0f;
        if (fadeDuration > 0f)
        {
            while (percent <= 1f)
            {
                percent += Time.fixedDeltaTime / fadeDuration;
                anim.CrossFade(motionName, fadeDuration);
                yield return waitForFixedUpdate;
            }
        }
        else
        {
            anim.Play(motionName, 0, 0);
        }

        anim.SetFloat(lastAnim, 0f);
        lastAnim = motionName;

        percent = 0f;
        while (percent <= 0.9999f)
        {
            percent += Time.fixedDeltaTime / fixedAnimationDuration;
            anim.SetFloat(motionName, percent);

            // if the loop is true, keep running the while loop
            if (loop && percent >= 0.9999f)
                percent = 0f;

            yield return waitForFixedUpdate;
        }

        currentAnim = "";
        isPlaying = false;
    }

    public void StopAnimation()
    {
        if (animationCor != null)
        {
            currentAnim = "";
            isPlaying = false;
            StopCoroutine(animationCor); // Stop the last animation.
        }
    }

    #endregion
}
