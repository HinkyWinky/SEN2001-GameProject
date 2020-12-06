using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    [Serializable] public class ActionDash : ActionLeaf
    {
        [SerializeField, Range(0.1f, 10f)] private float dashSpeed = 4f;
        [SerializeField, Range(0.15f, 50f)] private float stopDistance = 1f;
        [SerializeField, Range(0.01f, 0.2f)] private float errorDistanceThreshold = 0.1f;

        private Vector3 targetPos;
        private Vector3 destination;
        private Vector3 firstTargetPos;

        private IEnumerator dashToTargetPos;

        protected override NodeStates Action()
        {
            if (IsFirstLoop)
            {
                firstTargetPos = targetPos;
                destination = firstTargetPos + (brain.rig.position - firstTargetPos).normalized * stopDistance;

                NavMesh.SamplePosition(destination, out NavMeshHit hit, 10f, 1);
                if (hit.hit)
                    destination = hit.position;
                else
                    return NodeStates.FAILURE;

                brain.StartRotate(destination);

                float distance = Vector3.Distance(brain.rig.position, destination);
                float duration = (distance - errorDistanceThreshold) / dashSpeed;
                brain.animX.StartAnimation("Dash", duration, false, 0f);

                if (dashToTargetPos != null)
                    brain.StopCoroutine(dashToTargetPos);
                dashToTargetPos = CoroutineUtils.LerpRigidbody(brain.rig, destination, duration);
                brain.StartCoroutine(dashToTargetPos);

                return NodeStates.RUNNING;
            }

            float distanceToTargetPos = Vector3.Distance(brain.rig.position, destination);
            if (distanceToTargetPos < stopDistance + errorDistanceThreshold)
                return NodeStates.SUCCESS;

            return NodeStates.RUNNING;
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }
    }
}
