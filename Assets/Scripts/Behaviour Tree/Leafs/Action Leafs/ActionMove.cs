using System;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    [Serializable]
    public class ActionMove : ActionLeaf
    {
        private StateMachine Machine => btState.machine;

        [SerializeField, Range(-50f, 50f)] private float stopDistance = 1f;
        [SerializeField, Range(0.01f, 0.3f)] private float stopDistanceTolerance = 0.1f;
        [SerializeField, Range(0f, 1f)] private float targetMoveDistanceTolerance = 0.2f;
        [SerializeField, Min(1f)] private float maxLoopCountDuration = 5f;
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

        public override void StartLeaf(BehaviourTreeState behaviourTreeState)
        {
            base.StartLeaf(behaviourTreeState);
            cornerDistanceTolerance = 0.025f;
            loopCountLimit = maxLoopCountDuration / btState.EvaluateDeltaTime;
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
                Vector3 startPos = Machine.rig.position;
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

                    NavMesh.CalculatePath(startPos, targetDestination, NavMesh.AllAreas, Machine.checkPath);
                    if (btState.machine.checkPath.status != NavMeshPathStatus.PathComplete) return NodeStates.RUNNING;

                    // If there is no obstacles on the way, then move directly to the target position,
                    // else start following the path.
                    if (btState.machine.checkPath.corners.Length <= 2)
                    {
                        isDirectMove = true;
                        StartCoroutineLerpMove(targetDestination, targetDistance / Machine.moveSpeed);
                        StartCoroutineLerpRotate(targetDestination);
                    }
                    else
                    {
                        isDirectMove = false;
                        Machine.movePath = Machine.checkPath;
                        startCornerIndex = 0;
                        endCornerIndex = startCornerIndex + 1;
                        totalDistance = 0f;
                        for (int j = 0; j < btState.machine.movePath.corners.Length - 1; j++)
                            totalDistance += Vector3.Distance(btState.machine.movePath.corners[j], btState.machine.movePath.corners[j + 1]);
                        totalDuration = totalDistance / btState.machine.moveSpeed;
                        float cornerDistance = Vector3.Distance(Machine.movePath.corners[startCornerIndex], Machine.movePath.corners[endCornerIndex]);
                        float cornerDuration = totalDuration * cornerDistance / totalDistance;
                        StartCoroutineLerpMove(Machine.movePath.corners[endCornerIndex], cornerDuration);
                        StartCoroutineLerpRotate(Machine.movePath.corners[endCornerIndex]);
                    }
                    return NodeStates.RUNNING;
                }
            }

            // If AI is not following the path,
            if (isDirectMove)
            {
                // If AI is at the target destination, then return success,
                float distanceToTargetPos = Vector3.Distance(Machine.rig.position, targetDestination);
                if (distanceToTargetPos < Mathf.Abs(stopDistance + stopDistanceTolerance)) return NodeStates.SUCCESS;

                // else keep moving to the target destination.
                return NodeStates.RUNNING;
            }

            // If AI is following the path, then calculate the target path corner is near or not.
            // If the corner is near, then start moving to the next corner,
            // else keep moving to the corner.
            float distanceToCorner = Vector3.Distance(Machine.rig.position, Machine.movePath.corners[endCornerIndex]);
            // If this is the last corner, then return success, 
            if (endCornerIndex == Machine.movePath.corners.Length - 1)
            {
                if (distanceToCorner < Mathf.Abs(stopDistance + stopDistanceTolerance)) return NodeStates.SUCCESS;
            }
            // else keep moving next corner.
            if (distanceToCorner < cornerDistanceTolerance)
            {
                startCornerIndex += 1;
                endCornerIndex = startCornerIndex + 1;
                float cornerDistance = Vector3.Distance(Machine.movePath.corners[startCornerIndex], Machine.movePath.corners[endCornerIndex]);
                float cornerDuration = totalDuration * cornerDistance / totalDistance;
                StartCoroutineLerpMove(Machine.movePath.corners[endCornerIndex], cornerDuration);
                StartCoroutineLerpRotate(Machine.movePath.corners[endCornerIndex]);

                return NodeStates.RUNNING;
            }
            return NodeStates.RUNNING;
        }

        private void StartCoroutineLerpMove(Vector3 targetPos, float duration)
        {
            if (Machine.moveToTargetPos != null)
                Machine.StopCoroutine(Machine.moveToTargetPos);
            Machine.moveToTargetPos = CoroutineUtils.LerpMove(Machine.rig, targetPos, duration);
            Machine.StartCoroutine(Machine.moveToTargetPos);
        }
        private void StartCoroutineLerpRotate(Vector3 targetPos)
        {
            if (Machine.rotateToTargetPos != null)
                Machine.StopCoroutine(Machine.rotateToTargetPos);
            Machine.rotateToTargetPos = CoroutineUtils.LerpRotate(Machine.rig, targetPos, Machine.rotationDuration);
            Machine.StartCoroutine(Machine.rotateToTargetPos);
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            curtargetPos = targetPosition;
        }

        protected override void OnReset()
        {
            base.OnReset();
            lastFrameTargetPos = Vector3.zero;
            targetDestination = Vector3.zero;
            targetDistance = 0;
        }
    }
}
