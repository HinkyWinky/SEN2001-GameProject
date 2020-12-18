using System;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class ActionPlayAnimation : ActionLeaf
    {
        private StateMachine Machine => btState.machine;

        [SerializeField] private AnimData animData = default;

        protected override NodeStates Action()
        {
            if (IsFirstLoop)
            {
                Machine.animX.StartAnimation(animData);
                return NodeStates.RUNNING;
            }

            if (!Machine.animX.IsPlaying(animData.AnimName))
                return NodeStates.SUCCESS;

            return NodeStates.RUNNING;
        }
    }
}
