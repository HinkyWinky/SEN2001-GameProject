using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionAttack : ActionLeaf
    {
        [SerializeField, Range(0.05f, 50f)] private float attackDistance = 1f;
        [SerializeField, Range(0.01f, 0.2f)] private float attackDistanceTolerance = 0.1f;

        [SerializeField] private string animationMotionName = default;
        [SerializeField, Min(0)] private float animationDuration = 1f;
        [SerializeField, Range(0f, 1f)] private float animationFadeDuration = 0f;

        private Vector3 targetPos;

        protected override NodeStates Action()
        {
            if (IsFirstLoop)
            {
                float distanceToTargetPos = Vector3.Distance(brain.rig.transform.position, targetPos);
                if (distanceToTargetPos > attackDistance + attackDistanceTolerance)
                    return NodeStates.FAILURE;

                brain.animX.StartAnimation(animationMotionName, animationDuration, false, animationFadeDuration);
                return NodeStates.RUNNING;
            }

            if (!brain.animX.IsPlaying(animationMotionName))
                return NodeStates.SUCCESS;

            return NodeStates.RUNNING;
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }
    }
}
