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
}
