using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorX : MonoBehaviour
{
    [HideInInspector] public Animator anim;

    private IEnumerator animationCor;
    private IEnumerator animationSequenceCor;

    [ShowInInspector, ReadOnly] private bool isAnimPlaying;
    public bool IsAnimPlaying => isAnimPlaying;

    [ShowInInspector, ReadOnly] private string currentAnim = "Idle";
    public string CurrentAnim => currentAnim;
    [ShowInInspector, ReadOnly] private string lastAnim = "Idle";

    public List<AnimData> animsData = new List<AnimData>();
    public List<AnimSequenceData> animSequencesData = new List<AnimSequenceData>();

    public AnimData ReturnAnimData(string animName)
    {
        return animsData.Find(t => t.AnimName == animName);
    }
    public AnimSequenceData ReturnAnimSequenceData(string animSequenceName)
    {
        return animSequencesData.Find(t => t.SequenceName == animSequenceName);
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public bool IsPlaying(string animName)
    {
        return currentAnim == animName;
    }

    #region Play Animation
    public void StartAnimation(AnimData animData)
    {
        StopAnimation(true);
        animationCor = PlayAnimation(animData.AnimName, animData.duration, animData.isLoop, animData.normalizedFadeDuration);
        StartCoroutine(animationCor); // Start the new animation.
    }
    public void StartAnimation(AnimData animData, float duration)
    {
        StopAnimation(true);
        animationCor = PlayAnimation(animData.AnimName, duration, animData.isLoop, animData.normalizedFadeDuration);
        StartCoroutine(animationCor); // Start the new animation.
    }
    public void StartAnimation(string animName, float duration, bool loop, float normalizedFadeDuration)
    {
        StopAnimation(true);
        animationCor = PlayAnimation(animName, duration, loop, normalizedFadeDuration);
        StartCoroutine(animationCor); // Start the new animation.
    }
    private IEnumerator PlayAnimation(string animName, float animationDuration, bool loop, float normalizedFadeDuration)
    {
        float fixedAnimationDuration = animationDuration;
        float percent = 0f;

        if (isAnimPlaying && normalizedFadeDuration > 0f)
        {
            float fadeDuration = animationDuration * normalizedFadeDuration;
            fixedAnimationDuration = animationDuration - fadeDuration;
            while (percent < fadeDuration)
            {
                anim.CrossFade(animName, normalizedFadeDuration);
                percent += Time.deltaTime;
                yield return null;
            }
            anim.CrossFade(animName, normalizedFadeDuration);
        }
        else
        {
            anim.Play(animName, 0, 0);
        }

        yield return null;

        currentAnim = animName;
        isAnimPlaying = true;
        anim.SetFloat(lastAnim, 0f);
        lastAnim = animName;

        percent = 0f;
        while (percent < fixedAnimationDuration)
        {
            anim.SetFloat(currentAnim, percent / fixedAnimationDuration);
            percent += Time.deltaTime;
            // if the loop is true, keep running the while loop
            if (loop && (percent / fixedAnimationDuration >= 1f))
                percent = 0f;
            yield return null;
        }
        anim.SetFloat(currentAnim,1f);
        // animation is finished
        currentAnim = "";
        isAnimPlaying = false;
    }
    public void StopAnimation(bool stopSequence)
    {
        if (stopSequence)
            StopAnimationSequence();

        if (animationCor == null) return;
        currentAnim = "";
        StopCoroutine(animationCor); // Stop the last animation.
    }
    #endregion

    #region Play Animation Sequence
    public void StartAnimationSequence(string animSequenceName, float duration)
    {
        StopAnimation(true);
        animationSequenceCor = PlayAnimationSequence(animSequenceName, duration);
        StartCoroutine(animationSequenceCor); // Start the new animation.
    }
    private IEnumerator PlayAnimationSequence(string animSequenceName, float duration)
    {
        AnimSequenceData sequence = ReturnAnimSequenceData(animSequenceName);
        float totalLength = sequence.Length;

        for (int i = 0; i < sequence.animsData.Count; i++)
        {
            AnimData curAnimData = sequence.animsData[i];
            float durationForThisAnim = curAnimData.Length * duration / totalLength;

            float fixedAnimationDuration = durationForThisAnim;
            float percent = 0f;

            if (i == 0)
            {
                if (isAnimPlaying && sequence.normalizedFadeDuration > 0f)
                {
                    float fadeDuration = (durationForThisAnim * sequence.normalizedFadeDuration);
                    fixedAnimationDuration = durationForThisAnim - fadeDuration;
                    while (percent < fadeDuration)
                    {
                        anim.CrossFade(curAnimData.AnimName, curAnimData.normalizedFadeDuration);
                        percent += Time.deltaTime;
                        yield return null;
                    }
                    anim.CrossFade(curAnimData.AnimName, curAnimData.normalizedFadeDuration);
                }
                else
                    anim.Play(curAnimData.AnimName, 0, 0);
                yield return null;
            }
            else
                anim.Play(curAnimData.AnimName, 0, 0);

            currentAnim = sequence.animsData[i].AnimName;
            isAnimPlaying = true;
            anim.SetFloat(lastAnim, 0f);
            lastAnim = currentAnim;

            percent = 0f;
            while (percent < fixedAnimationDuration)
            {
                anim.SetFloat(currentAnim, percent / fixedAnimationDuration);
                percent += Time.deltaTime;
                yield return null;
            }
            anim.SetFloat(currentAnim, 1f);

            // animation is finished
            if (i == sequence.animsData.Count - 1)
            {
                currentAnim = "";
                isAnimPlaying = false;
            }

            yield return null;
        }
    }
    public void StopAnimationSequence()
    {
        if (animationSequenceCor == null) return;
        currentAnim = "";
        StopCoroutine(animationSequenceCor); // Stop the last animation sequence.
    }
    #endregion
}
