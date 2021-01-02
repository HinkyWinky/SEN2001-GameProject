using System;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class ActionPlayAnimation : ActionLeaf
    {
        [SerializeField] private AnimData animData = default;
        [SerializeField] private bool waitUntilFinish = false;

        protected override NodeStates Action()
        {
            if (IsFirstLoop)
            {
                Machine.animX.StartAnimation(animData);
                return waitUntilFinish ? NodeStates.RUNNING : NodeStates.SUCCESS;
            }

            if (!Machine.animX.IsPlaying(animData.AnimName))
                return NodeStates.SUCCESS;

            return NodeStates.RUNNING;
        }
    }
}
