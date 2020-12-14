using System.Collections;
using Game.AI;
using UnityEngine;
using UnityEngine.AI;

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
   
    public static IEnumerator LerpMove(Rigidbody rigidbody, Vector3 targetPos, float duration)
    {
        Vector3 startPos = rigidbody.position;
        float percent = 0;
        while (percent < duration)
        {
            Debug.DrawLine(startPos, targetPos, Color.cyan);
            rigidbody.position = Vector3.Lerp(startPos, targetPos, percent / duration);
            percent += Time.fixedDeltaTime;
            yield return waitForFixedUpdate;
        }
        rigidbody.position = targetPos;
        yield return waitForFixedUpdate;
    }

    public static IEnumerator LerpRotate(Rigidbody rigidbody, Vector3 targetPos, float rotationDuration)
    {
        Vector3 fixedTargetPos = new Vector3(targetPos.x, rigidbody.position.y, targetPos.z); // Target`s Position on x and z axis.
        Vector3 forward = Vector3.Normalize(fixedTargetPos - rigidbody.position); // Direction from the player`s position to the target`s position.
        if (forward == Vector3.zero) yield break;

        var startRot = rigidbody.rotation;
        var targetRot = Quaternion.LookRotation(forward, Vector3.up); // Calculate target rotation value.
        float fixedDuration = rotationDuration * Quaternion.Angle(startRot, targetRot) / 180f; // Calculate the duration value needed to rotate to the target angle.
        float percent = 0f;
        while (percent < fixedDuration)
        {
            rigidbody.rotation = Quaternion.Slerp(startRot, targetRot, percent / fixedDuration); // Rotate the player to the target rotation value.
            percent += Time.deltaTime;
            yield return CoroutineUtils.waitForFixedUpdate;
        }
        rigidbody.rotation = targetRot;
        yield return CoroutineUtils.waitForFixedUpdate;
    }
}
