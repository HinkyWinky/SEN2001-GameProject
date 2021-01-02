using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI
{
    [Serializable]
    public class ActionMove : ActionLeaf
    {
        [SerializeField, Range(-50f, 50f)] private float stopDistance = 1f;
        [SerializeField, Range(0.01f, 0.05f)] private float stopDistanceTolerance = 0.025f;
        [SerializeField, Range(0f, 1f)] private float targetMoveDistanceTolerance = 0.2f;
        [SerializeField, Min(1f)] private float maxLoopCountDuration = 5f;
        [SerializeField] private AnimData moveAnimData = default;
        private enum RotationTypes { DESTINATION, TARGET };
        [SerializeField] private RotationTypes rotationType = default;

        private Vector3 targetTransformPos, lastEvaluateTargetPos, targetDestination;
        private bool isDirectMove, isFirstUpdate, isFirstDoor;
        private int startCornerIndex, endCornerIndex;
        private float targetDistance, cornerDistanceTolerance, totalDuration, totalDistance, loopCountLimit;

        public override void StartLeaf(BehaviourTreeState behaviourTreeState)
        {
            base.StartLeaf(behaviourTreeState);
            cornerDistanceTolerance = 0.1f;
            loopCountLimit = maxLoopCountDuration / btState.EvaluateDeltaTime;
        }

        protected override NodeStates Action()
        {
            // If the action is looping more than loop count limit, then stop looping,
            // and evaluate behaviour tree again.
            if (loopCount > loopCountLimit) return NodeStates.FAILURE;
            // If the current target`s position is not equal to the target`s position of last frame,
            // calculate target destination again.
            if (lastEvaluateTargetPos != targetTransformPos) 
            {
                Vector3 startPos = Machine.rig.position;
                lastEvaluateTargetPos = targetTransformPos;
                Vector3 dirToAiFromTargetPos = (startPos - lastEvaluateTargetPos).normalized;
                Vector3 destination = lastEvaluateTargetPos + dirToAiFromTargetPos * stopDistance;

                NavMesh.SamplePosition(destination, out NavMeshHit hit, 10f, 1);
                if (hit.hit) destination = hit.position;
                else return NodeStates.FAILURE; // If there is no reachable position for the AI.

                // Calculate the distance between the start position and the target destination.
                float distance = Vector3.Distance(startPos, destination);
                // If the calculated distance is greater than move distance tolerance,
                // then move to the new destination, else keep moving previous last target destination.
                if (Math.Abs(distance - targetDistance) > targetMoveDistanceTolerance)
                {
                    NavMesh.CalculatePath(startPos, destination, NavMesh.AllAreas, Machine.checkPath);
                    if (btState.machine.checkPath.status != NavMeshPathStatus.PathComplete) return NodeStates.FAILURE;

                    // If there is no obstacles on the way, then move directly to the target position,
                    if (btState.machine.checkPath.corners.Length <= 2)
                    {
                        targetDistance = distance;
                        targetDestination = destination;
                        isDirectMove = true;
                        StartCoroutineLerpMove(targetDestination, targetDistance / Machine.moveSpeed);
                        if (rotationType == RotationTypes.DESTINATION)
                            Machine.SetRotationTargetPos(targetDestination);
                    }
                    else // else start following the path.
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
                        Machine.SetRotationTargetPos(Machine.movePath.corners[endCornerIndex]);
                    }
                    return NodeStates.RUNNING;
                }
            }

            // If there is a door on the way, wait until opened.
            if (Machine.hasDoor && !Machine.door.IsOpen)
            {
                if (!isFirstDoor)
                {
                    isFirstDoor = true;
                    Machine.door.OpenDoor();
                    return NodeStates.RUNNING;
                }
                return NodeStates.RUNNING;
            }
            if (isFirstDoor)
                isFirstDoor = false;

            // If AI is at the target destination, then return success.
            float distanceToTargetPos = Vector3.Distance(Machine.rig.position, targetDestination);
            if (distanceToTargetPos < Mathf.Abs(stopDistance + stopDistanceTolerance)) return NodeStates.SUCCESS;

            // If AI is not following the path and moving to target destination directly, then keep moving.
            if (isDirectMove) return NodeStates.RUNNING;

            if (endCornerIndex == Machine.movePath.corners.Length) return NodeStates.RUNNING;
            // If AI is following the path, then calculate the target path corner is near or not.
            // If the corner is near, then start moving to the next corner,
            // else keep moving to the current corner.
            float distanceToCorner = Vector3.Distance(Machine.rig.position, Machine.movePath.corners[endCornerIndex]);
            if (distanceToCorner < cornerDistanceTolerance)
            {
                startCornerIndex += 1;
                endCornerIndex = startCornerIndex + 1;
                float cornerDistance = Vector3.Distance(Machine.movePath.corners[startCornerIndex], Machine.movePath.corners[endCornerIndex]);
                float cornerDuration = totalDuration * cornerDistance / totalDistance;
                StartCoroutineLerpMove(Machine.movePath.corners[endCornerIndex], cornerDuration);
                Machine.SetRotationTargetPos(Machine.movePath.corners[endCornerIndex]);
                return NodeStates.RUNNING;
            }
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
            targetTransformPos = targetPosition;

            if (NodeState != NodeStates.RUNNING) return;

            if (!isFirstUpdate)
            {
                isFirstUpdate = true;
                if (Machine.animX.CurrentAnim != moveAnimData.AnimName)
                    Machine.animX.StartAnimation(moveAnimData);
            }

            if (isDirectMove)
            {
                if (rotationType == RotationTypes.TARGET)
                    Machine.SetRotationTargetPos(targetTransformPos);
            }
        }

        public override void OnReset()
        {
            base.OnReset();
            Machine.movePath.ClearCorners();
            isDirectMove = false;
            isFirstUpdate = false;
            lastEvaluateTargetPos = Vector3.zero;
            targetDestination = Vector3.zero;
            targetDistance = 0;
            startCornerIndex = 0;
            endCornerIndex = 0;
            isFirstDoor = false;
        }
    }
}
