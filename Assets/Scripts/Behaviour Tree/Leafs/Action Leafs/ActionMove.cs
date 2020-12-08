using System;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    [Serializable]
    public class ActionMove : ActionLeaf
    {
        [SerializeField, Range(-50f, 50f)] private float stopDistance = 1f;
        [SerializeField, Range(0.01f, 0.3f)] private float stopDistanceTolerance = 0.1f;
        [SerializeField, Range(0f, 1f)] private float targetMoveDistanceTolerance = 0.2f;
        [SerializeField, Min(1f)] private float maxLoopCountDuration = 10f;
        private Vector3 curtargetPos;
        private Vector3 lastFrameTargetPos;
        private Vector3 targetDestination;
        private float targetDistance;

        private int startCornerIndex;
        private int endCornerIndex;
        private float cornerDistanceTolerance;
        private float totalDuration;
        private float totalDistance;
        private bool isDirectMove;
        private float loopCountLimit;

        public override void StartLeaf(BehaviourBrain behaviourBrain)
        {
            base.StartLeaf(behaviourBrain);
            cornerDistanceTolerance = 0.1f;
            loopCountLimit = maxLoopCountDuration / brain.EvaluateDeltaTime;
        }

        protected override NodeStates Action()
        {
            // If the action is looping more than loop count limit, then stop looping,
            // and evaluate behaviour tree again.
            if (loopCount > loopCountLimit) return NodeStates.FAILURE;
            // If the current target`s position is not equal to the target`s position of last frame,
            // calculate target destination again.
            if (lastFrameTargetPos != curtargetPos) 
            {
                Vector3 startPos = brain.rig.position;
                lastFrameTargetPos = curtargetPos;
                Vector3 dirToAiFromTargetPos = (startPos - lastFrameTargetPos).normalized;
                Vector3 curDestination = lastFrameTargetPos + dirToAiFromTargetPos * stopDistance;

                NavMesh.SamplePosition(curDestination, out NavMeshHit hit, 10f, 1);
                if (hit.hit) curDestination = hit.position;
                else return NodeStates.FAILURE; // If there is no reachable position for the AI.

                // Calculate the distance between the start position and the target destination.
                float curDistance = Vector3.Distance(startPos, curDestination);
                // If the calculated distance is greater than move distance tolerance,
                // then move to the new destination, else keep moving previous last target destination.
                if (Math.Abs(curDistance - targetDistance) > targetMoveDistanceTolerance)
                {
                    targetDistance = curDistance;
                    targetDestination = curDestination;

                    NavMesh.CalculatePath(startPos, targetDestination, NavMesh.AllAreas, brain.checkPath);
                    if (brain.checkPath.status != NavMeshPathStatus.PathComplete) return NodeStates.RUNNING;

                    // If there is no obstacles on the way, then move directly to the target position,
                    // else start following the path.
                    if (brain.checkPath.corners.Length <= 2)
                    {
                        isDirectMove = true;
                        StartCoroutineLerpMove(targetDestination, targetDistance / brain.moveSpeed);
                        StartCoroutineLerpRotate(targetDestination);
                    }
                    else
                    {
                        isDirectMove = false;
                        brain.movePath = brain.checkPath;
                        startCornerIndex = 0;
                        endCornerIndex = startCornerIndex + 1;
                        totalDistance = 0f;
                        for (int j = 0; j < brain.movePath.corners.Length - 1; j++)
                            totalDistance += Vector3.Distance(brain.movePath.corners[j], brain.movePath.corners[j + 1]);
                        totalDuration = totalDistance / brain.moveSpeed;
                        float cornerDistance = Vector3.Distance(brain.movePath.corners[startCornerIndex], brain.movePath.corners[endCornerIndex]);
                        float cornerDuration = totalDuration * cornerDistance / totalDistance;
                        StartCoroutineLerpMove(brain.movePath.corners[endCornerIndex], cornerDuration);
                        StartCoroutineLerpRotate(brain.movePath.corners[endCornerIndex]);
                    }
                    return NodeStates.RUNNING;
                }
            }

            // If AI is at the target destination, then return success,
            // else keep moving to the target destination.
            float distanceToTargetPos = Vector3.Distance(brain.rig.position, targetDestination);
            if (distanceToTargetPos < Mathf.Abs(stopDistance + stopDistanceTolerance)) return NodeStates.SUCCESS;

            // If AI is not following the path, then keep moving directly to the target position.
            if (isDirectMove) return NodeStates.RUNNING;

            // If AI is following the path, then calculate the target path corner is near or not.
            // If the corner is near, then start moving to the next corner,
            // else keep moving to the corner.
            float distanceToCorner = Vector3.Distance(brain.rig.position, brain.movePath.corners[endCornerIndex]);
            if (distanceToCorner < cornerDistanceTolerance)
            {
                if (endCornerIndex == brain.movePath.corners.Length - 1) return NodeStates.SUCCESS;

                startCornerIndex += 1;
                endCornerIndex = startCornerIndex + 1;
                float cornerDistance = Vector3.Distance(brain.movePath.corners[startCornerIndex], brain.movePath.corners[endCornerIndex]);
                float cornerDuration = totalDuration * cornerDistance / totalDistance;
                StartCoroutineLerpMove(brain.movePath.corners[endCornerIndex], cornerDuration);
                StartCoroutineLerpRotate(brain.movePath.corners[endCornerIndex]);

                return NodeStates.RUNNING;
            }
            return NodeStates.RUNNING;
        }
        private void StartCoroutineLerpMove(Vector3 targetPos, float duration)
        {
            if (brain.moveToTargetPos != null)
                brain.StopCoroutine(brain.moveToTargetPos);
            brain.moveToTargetPos = CoroutineUtils.LerpMove(brain.rig, targetPos, duration);
            brain.StartCoroutine(brain.moveToTargetPos);
        }
        private void StartCoroutineLerpRotate(Vector3 targetPos)
        {
            if (brain.rotateToTargetPos != null)
                brain.StopCoroutine(brain.rotateToTargetPos);
            brain.rotateToTargetPos = CoroutineUtils.LerpRotate(brain.rig, targetPos, brain.rotationDuration);
            brain.StartCoroutine(brain.rotateToTargetPos);
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            curtargetPos = targetPosition;
        }
    }
}
