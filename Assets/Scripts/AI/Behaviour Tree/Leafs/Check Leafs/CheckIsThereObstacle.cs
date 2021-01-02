using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI
{
    [Serializable]
    public class CheckIsThereObstacle : CheckLeaf
    {
        private enum CheckRayComparision { HIT, EMPTY };

        [SerializeField, Tooltip("Choosen type returns success.")]
        private CheckRayComparision checkRayComparision = default;
        [SerializeField] private float stateMachineOffset = 0f;
        [SerializeField] private float destinationOffset = 0f;
        [SerializeField, Range(-50f, 50f)] private float stopDistance = 1f;

        private Vector3 endPos;

        public override NodeStates Check()
        {
            Vector3 startPos = Machine.rig.position + Vector3.up * stateMachineOffset;
            Vector3 destination = endPos + (startPos - endPos).normalized * stopDistance + Vector3.up * destinationOffset;
            Vector3 direction = destination - startPos;
            float distance = Vector3.Distance(startPos, destination);

            if (Physics.Raycast(startPos, direction, distance))
            {
                switch (checkRayComparision)
                {
                    case CheckRayComparision.HIT:
                        return NodeStates.SUCCESS;
                    case CheckRayComparision.EMPTY:
                        return NodeStates.FAILURE;
                }
            }
            else
            {
                switch (checkRayComparision)
                {
                    case CheckRayComparision.HIT:
                        return NodeStates.FAILURE;
                    case CheckRayComparision.EMPTY:
                        return NodeStates.SUCCESS;
                }
            }

            Debug.LogWarning("Logic Error");
            return NodeStates.FAILURE;
        }

        public void UpdateLeaf(Vector3 targetPosition)
        {
            endPos = targetPosition;
        }
    }
}

