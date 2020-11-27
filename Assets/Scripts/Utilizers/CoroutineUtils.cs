using System.Collections;
using UnityEngine;

public static class CoroutineUtils
{
    public static IEnumerator WaitForSeconds(float seconds)
    {
        float startTime = Time.time;
        do
        {
            yield return null;
        } while (Time.time < startTime + seconds);
    }

    public static IEnumerator WaitForSecondsRealtime(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;
        do
        {
            yield return null;
        } while (Time.realtimeSinceStartup < startTime + seconds);
    }

    public static IEnumerator MoveTowards(Transform objectToMove, Vector3 targetPos, float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            Vector3 currentPos = objectToMove.position;
            float time = Vector3.Distance(currentPos, targetPos) / (duration - counter) * Time.deltaTime;
            objectToMove.position = Vector3.MoveTowards(currentPos, targetPos, time);
            yield return null;
        }
    }

    public static IEnumerator Lerp(Transform objectToMove, Vector3 targetPos, float duration)
    {
        Vector3 startPos = objectToMove.position;
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            objectToMove.position = Vector3.Lerp(startPos, targetPos, counter / duration);
            yield return null;
        }
    }
}
