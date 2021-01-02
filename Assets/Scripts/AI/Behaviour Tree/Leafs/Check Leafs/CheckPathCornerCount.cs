using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI
{
    [Serializable]
    public class CheckPathCornerCount : CheckLeaf
    {
        [SerializeField, Tooltip("Greater => inclusive, Smaller => exclusive")]
        private CheckType checkType = default;
        [SerializeField, Min(0)] private int cornerCount = 0;
        [SerializeField, Range(-50f, 50f)] private float stopDistance = 1f;
        private Vector3 targetPos;

        public override NodeStates Check()
        {
            Vector3 startPos = Machine.rig.position;
            Vector3 destination = targetPos + (startPos - targetPos).normalized * stopDistance;

            NavMesh.SamplePosition(destination, out NavMeshHit hit, 10f, 1);
            if (hit.hit)
                destination = hit.position;
            else
                return NodeStates.FAILURE;

            NavMesh.CalculatePath(startPos, destination, NavMesh.AllAreas, Machine.checkPath);
            if (Machine.checkPath.status == NavMeshPathStatus.PathComplete)
            {
                int cornersLength = Machine.checkPath.corners.Length;
                Machine.checkPath.ClearCorners();
                switch (checkType)
                {
                    case CheckType.IS_GREATER:
                        return cornersLength >= cornerCount ? NodeStates.SUCCESS : NodeStates.FAILURE;
                    case CheckType.IS_SMALLER:
                        return cornersLength < cornerCount ? NodeStates.SUCCESS : NodeStates.FAILURE;
                    case CheckType.IS_EQUAL:
                        return cornersLength == cornerCount ? NodeStates.SUCCESS : NodeStates.FAILURE;
                }
            }

            return NodeStates.FAILURE;
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }
    }
}
