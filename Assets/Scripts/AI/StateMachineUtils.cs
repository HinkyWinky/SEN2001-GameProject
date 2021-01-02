using System.Collections;
using UnityEngine;

namespace Game.AI
{
    public static class StateMachineUtils
    {
        public static YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

        public static IEnumerator LerpMove(StateMachine machine, Vector3 targetPos, float duration)
        {
            Vector3 startPos = machine.rig.position;
            float percent = 0;
            while (percent < duration)
            {
                while (machine.hasDoor)
                    yield return waitForFixedUpdate;

                Debug.DrawLine(startPos, targetPos, Color.cyan);
                machine.rig.position = Vector3.Lerp(startPos, targetPos, percent / duration);
                percent += Time.fixedDeltaTime;
                yield return waitForFixedUpdate;
            }
            machine.rig.position = targetPos;
            yield return waitForFixedUpdate;
        }

        public static IEnumerator LerpRotate(StateMachine machine, Vector3 targetPos, float rotationDuration)
        {
            Vector3 fixedTargetPos = new Vector3(targetPos.x, machine.rig.position.y, targetPos.z); // Target`s Position on x and z axis.
            Vector3 forward = Vector3.Normalize(fixedTargetPos - machine.rig.position); // Direction from the player`s position to the target`s position.
            if (forward == Vector3.zero) yield break;

            var startRot = machine.rig.rotation;
            var targetRot = Quaternion.LookRotation(forward, Vector3.up); // Calculate target rotation value.
            float fixedDuration = rotationDuration * Quaternion.Angle(startRot, targetRot) / 180f; // Calculate the duration value needed to rotate to the target angle.
            float percent = 0f;
            while (percent < fixedDuration)
            {
                machine.rig.rotation = Quaternion.Slerp(startRot, targetRot, percent / fixedDuration); // Rotate the player to the target rotation value.
                percent += Time.fixedDeltaTime;
                yield return waitForFixedUpdate;
            }
            machine.rig.rotation = targetRot;
            yield return waitForFixedUpdate;
        }
    }
}
