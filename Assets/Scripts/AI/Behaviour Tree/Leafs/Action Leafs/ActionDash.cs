using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI
{
    [Serializable] public class ActionDash : ActionLeaf
    {
        [SerializeField, Range(0.1f, 10f)] private float dashSpeed = 4f;
        [SerializeField, Range(-50f, 50f)] private float stopDistance = 1f;
        [SerializeField, Range(0.01f, 0.05f)] private float stopDistanceTolerance = 0.025f;
        [SerializeField] private AnimData dashAnimData = default;

        private Vector3 curTargetPos, lastFrameTargetPos, targetDestination;

        protected override NodeStates Action()
        {
            // If the evaluation of the last frame is not running.
            if (IsFirstLoop)
            {
                Vector3 startPos = Machine.rig.position;
                lastFrameTargetPos = curTargetPos;
                Vector3 dirToAiFromTargetPos = (startPos - lastFrameTargetPos).normalized;
                targetDestination = lastFrameTargetPos + dirToAiFromTargetPos * stopDistance;

                NavMesh.SamplePosition(targetDestination, out NavMeshHit hit, 10f, 1);
                if (hit.hit) targetDestination = hit.position;
                else return NodeStates.FAILURE; // If there is no reachable position for the AI.

                // Calculate the distance between the start position and the target destination.
                float distance = Vector3.Distance(startPos, targetDestination);
                float duration = distance / dashSpeed;

                StartCoroutineLerpMove(targetDestination, duration);
                Machine.SetRotationTargetPos(targetDestination);
                Machine.animX.StartAnimation(dashAnimData, duration);
                return NodeStates.RUNNING;
            }

            // If AI is at the target destination, then return success,
            // else keep moving to the target destination.
            float distanceToTargetPos = Vector3.Distance(Machine.rig.position, targetDestination);
            if (distanceToTargetPos <= Mathf.Abs(stopDistance + stopDistanceTolerance)) return NodeStates.SUCCESS;

            return NodeStates.RUNNING;
        }
        private void StartCoroutineLerpMove(Vector3 targetPos, float duration)
        {
            if (Machine.action != null)
                Machine.StopCoroutine(Machine.action);
            Machine.action = StateMachineUtils.LerpMove(Machine, targetPos, duration);
            Machine.StartCoroutine(Machine.action);
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            curTargetPos = targetPosition;
        }
    }
}
