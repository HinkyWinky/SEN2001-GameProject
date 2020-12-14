using System;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class ActionAttack : ActionLeaf
    {
        private StateMachine Machine => btState.machine;

        [SerializeField, Range(0f, 360f)] private float attackAngleRange = 180f;
        [SerializeField, Range(0.05f, 50f)] private float maxAttackDistance = 1f;
        [SerializeField, Range(0.01f, 0.05f)] private float maxAttackDistanceTolerance = 0.025f;

        [SerializeField] private string animationMotionName = default;
        [SerializeField, Min(0)] private float animationDuration = 1f;
        [SerializeField, Range(0f, 1f)] private float animationFadeDuration = 0f;

        private Vector3 targetPos;

        protected override NodeStates Action()
        {
            if (IsFirstLoop)
            {
                if (CalculateAngleBetweenTarget() > attackAngleRange / 2f) return NodeStates.FAILURE;

                float distanceToTargetPos = Vector3.Distance(Machine.rig.position, targetPos);
                if (distanceToTargetPos > maxAttackDistance + maxAttackDistanceTolerance) return NodeStates.FAILURE;

                Machine.animX.StartAnimation(animationMotionName, animationDuration, false, animationFadeDuration);

                return NodeStates.RUNNING;
            }

            if (!Machine.animX.IsPlaying(animationMotionName)) return NodeStates.SUCCESS;

            return NodeStates.RUNNING;
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }

        private float CalculateAngleBetweenTarget()
        {
            Vector3 directionToTarget = (targetPos - Machine.rig.position).normalized;
            return Vector3.Angle(directionToTarget, Machine.rig.transform.forward);
        }
    }
}
