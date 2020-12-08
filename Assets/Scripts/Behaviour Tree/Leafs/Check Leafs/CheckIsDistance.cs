using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class CheckIsDistance : CheckLeaf
    {
        [SerializeField] private CheckType checkType = default;
        [SerializeField, ShowIf("checkType", CheckType.IS_EQUAL), Range(0.05f, 1f)]
        private float equalThreshold = 0.1f;
        [SerializeField, Range(0.1f, 50f)] private float distanceValue = 4f;
        private Vector3 targetPos;

        protected override  NodeStates Check()
        {
            float targetDistance = Vector3.Distance(brain.rig.position, targetPos);
            switch (checkType)
            {
                case CheckType.IS_GREATER:
                    return targetDistance > distanceValue ? NodeStates.SUCCESS : NodeStates.FAILURE;
                case CheckType.IS_SMALLER:
                    return targetDistance < distanceValue ? NodeStates.SUCCESS : NodeStates.FAILURE;
                case CheckType.IS_EQUAL:
                    if (targetDistance  < distanceValue + equalThreshold && targetDistance > distanceValue - equalThreshold)
                        return NodeStates.SUCCESS;
                    else
                        return NodeStates.FAILURE;
                default:
                    return NodeStates.RUNNING;
            }
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }
    }
}
