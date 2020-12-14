using System;
using System.Collections;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class ActionTimer : ActionLeaf
    {
        private StateMachine Machine => btState.machine;

        [SerializeField, Min(0)] private float duration = 0f;

        private WaitForSeconds waitForDuration;
        private bool isWaiting = false;

        public override void StartLeaf(BehaviourTreeState behaviourTreeState)
        {
            base.StartLeaf(behaviourTreeState);
            waitForDuration = new WaitForSeconds(duration);
        }

        protected override NodeStates Action()
        {
            if (IsFirstLoop)
            {
                if (Machine.action != null)
                    Machine.StopCoroutine(Machine.action);
                Machine.action = Timer();
                Machine.StartCoroutine(Machine.action);

                return NodeStates.RUNNING;
            }

            return isWaiting ? NodeStates.RUNNING : NodeStates.SUCCESS;
        }

        private IEnumerator Timer()
        {
            isWaiting = true;
            yield return waitForDuration;
            isWaiting = false;
        }

        protected override void OnReset()
        {
            base.OnReset();
            isWaiting = false;
        }
    }
}
