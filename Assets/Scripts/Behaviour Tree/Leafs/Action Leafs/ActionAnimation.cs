using System;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class ActionAnimation : ActionLeaf
    {
        private StateMachine Machine => btState.machine;

        [SerializeField] private string animationMotionName = default;
        [SerializeField, Min(0)] private float animationDuration = 1f;
        [SerializeField] private bool loop = false;
        [SerializeField, Range(0f, 1f)] private float animationFadeDuration = 0f;

        protected override NodeStates Action()
        {
            if (IsFirstLoop)
            {
                Machine.animX.StartAnimation(animationMotionName, animationDuration, loop, animationFadeDuration);
                return NodeStates.RUNNING;
            }

            if (!Machine.animX.IsPlaying(animationMotionName))
                return NodeStates.SUCCESS;

            return NodeStates.RUNNING;
        }
    }
}
