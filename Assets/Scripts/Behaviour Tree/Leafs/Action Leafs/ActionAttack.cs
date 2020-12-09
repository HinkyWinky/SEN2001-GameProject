using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionAttack : ActionLeaf
    {
        private StateMachine Machine => btState.machine;

        [SerializeField, Range(0.05f, 50f)] private float maxAttackDistance = 1f;
        [SerializeField, Range(0.01f, 0.2f)] private float maxAttackDistanceTolerance = 0.1f;

        [SerializeField] private string animationMotionName = default;
        [SerializeField, Min(0)] private float animationDuration = 1f;
        [SerializeField, Range(0f, 1f)] private float animationFadeDuration = 0f;

        private Vector3 targetPos;

        protected override NodeStates Action()
        {
            if (IsFirstLoop)
            {
                float distanceToTargetPos = Vector3.Distance(Machine.rig.transform.position, targetPos);
                if (distanceToTargetPos > maxAttackDistance + maxAttackDistanceTolerance)
                    return NodeStates.FAILURE;

                Machine.animX.StartAnimation(animationMotionName, animationDuration, false, animationFadeDuration);
                return NodeStates.RUNNING;
            }

            if (!Machine.animX.IsPlaying(animationMotionName))
                return NodeStates.SUCCESS;

            return NodeStates.RUNNING;
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }
    }
}
