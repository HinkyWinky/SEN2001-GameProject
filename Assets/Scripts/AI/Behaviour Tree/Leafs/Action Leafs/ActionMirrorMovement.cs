using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class ActionMirrorMovement : ActionLeaf
    {
        [SerializeField, Min(0)] private float duration = 0f;
        [SerializeField] private CheckIsDistance checkDistance = default;

        private bool isWaiting = false;
        private bool condition = false;

        private Rigidbody targetRig;
        private Vector3 axisInputs;

        public override void StartLeaf(BehaviourTreeState behaviourTreeState)
        {
            base.StartLeaf(behaviourTreeState);
            checkDistance.StartLeaf(behaviourTreeState);
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

            if (condition) return NodeStates.SUCCESS;

            return isWaiting ? NodeStates.RUNNING : NodeStates.FAILURE;
        }

        private IEnumerator Timer()
        {
            Machine.rig.isKinematic = false;
            isWaiting = true;
            float percent = 0;
            while (percent < duration)
            {
                if (checkDistance.Evaluate() == NodeStates.SUCCESS)
                {
                    Machine.rig.isKinematic = true;
                    condition = true;
                    yield break;
                }

                Machine.SetRotationTargetPos(targetRig.position);
                // machine moves in the opposite direction of the target.
                if (Mathf.Abs(axisInputs.z) < 0.02f)
                {
                    Machine.rig.velocity = -targetRig.velocity.normalized * Machine.moveSpeed / 2f;
                }

                percent += Time.fixedDeltaTime;
                yield return StateMachineUtils.waitForFixedUpdate;
            }
            Machine.rig.isKinematic = true;
            isWaiting = false;
        }

        public void UpdateLeaf(Rigidbody targetRigidbody, Vector3 userAxisInputs)
        {
            targetRig = targetRigidbody;
            checkDistance.UpdateLeaf(targetRig.position);
            axisInputs = userAxisInputs;
        }

        public override void OnReset()
        {
            base.OnReset();
            isWaiting = false;
            condition = false;
            checkDistance.OnReset();
        }
    }
}
