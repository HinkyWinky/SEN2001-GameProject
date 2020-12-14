using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class CheckIsDistance : CheckLeaf
    {
        private StateMachine Machine => btState.machine;

        [SerializeField] private CheckType checkType = default;
        [SerializeField, ShowIf("checkType", CheckType.IS_EQUAL), Range(0.01f, 0.05f)]
        private float equalThreshold = 0.025f;
        [SerializeField, Range(0.1f, 50f)] private float distanceValue = 4f;
        private Vector3 targetPos;

        protected override  NodeStates Check()
        {
            float distanceToTargetPos = Vector3.Distance(Machine.rig.position, targetPos);
            switch (checkType)
            {
                case CheckType.IS_GREATER:
                    return distanceToTargetPos >= distanceValue ? NodeStates.SUCCESS : NodeStates.FAILURE;
                case CheckType.IS_SMALLER:
                    return distanceToTargetPos < distanceValue ? NodeStates.SUCCESS : NodeStates.FAILURE;
                case CheckType.IS_EQUAL:
                    if (distanceToTargetPos  < distanceValue + equalThreshold && distanceToTargetPos > distanceValue - equalThreshold)
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
