using System.Collections;
using UnityEngine;

namespace Game
{
    public static class Lerp
    {
        public static YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

        public static IEnumerator MoveTo(Rigidbody rigidbody, Vector3 targetPos, float duration)
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

        public static IEnumerator LookAt(Rigidbody rigidbody, Vector3 targetPos, float rotationDuration)
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
                yield return waitForFixedUpdate;
            }

            rigidbody.rotation = targetRot;
            yield return waitForFixedUpdate;
        }

        public static IEnumerator MoveTo(Transform transform, Vector3 targetPos, float duration)
        {
            Vector3 startPos = transform.position;
            float percent = 0;
            while (percent < duration)
            {
                Debug.DrawLine(startPos, targetPos, Color.cyan);
                transform.position = Vector3.Lerp(startPos, targetPos, percent / duration);
                percent += Time.fixedDeltaTime;
                yield return null;
            }

            transform.position = targetPos;
            yield return null;
        }

        public static IEnumerator LookAt(Transform transform, Vector3 targetPos, float rotationDuration)
        {
            Vector3 fixedTargetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z); // Target`s Position on x and z axis.
            Vector3 forward = Vector3.Normalize(fixedTargetPos - transform.position); // Direction from the player`s position to the target`s position.
            if (forward == Vector3.zero) yield break;

            var startRot = transform.rotation;
            var targetRot = Quaternion.LookRotation(forward, Vector3.up); // Calculate target rotation value.
            float fixedDuration = rotationDuration * Quaternion.Angle(startRot, targetRot) / 180f; // Calculate the duration value needed to rotate to the target angle.
            float percent = 0f;
            while (percent < fixedDuration)
            {
                transform.rotation = Quaternion.Slerp(startRot, targetRot, percent / fixedDuration); // Rotate the player to the target rotation value.
                percent += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRot;
            yield return null;
        }
    }
}
