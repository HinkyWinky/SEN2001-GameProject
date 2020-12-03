using System.Collections;
using UnityEngine;

public static class CoroutineUtils
{
    public static YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

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

    public static IEnumerator MoveTowardsRigidbody(Rigidbody rigidbody, Vector3 targetPos, float duration)
    {
        float percent = 0;
        while (percent < duration)
        {
            percent += Time.fixedDeltaTime;
            Vector3 currentPos = rigidbody.position;
            float time = Vector3.Distance(currentPos, targetPos) / (duration - percent) * Time.deltaTime;
            rigidbody.position = Vector3.MoveTowards(currentPos, targetPos, time);
            yield return waitForFixedUpdate;
        }
    }
   
    public static IEnumerator LerpRigidbody(Rigidbody rigidbody, Vector3 targetPos, float speed)
    {
        Vector3 startPos = rigidbody.position;
        float percent = 0;
        while (percent <= 1f)
        {
            percent += Time.fixedDeltaTime * speed;
            if (percent > 1)
                percent = 1f;
            rigidbody.position = Vector3.Lerp(startPos, targetPos, percent);
            yield return waitForFixedUpdate;
        }
    }
}
