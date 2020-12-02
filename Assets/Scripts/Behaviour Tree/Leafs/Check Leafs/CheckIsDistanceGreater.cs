using System;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    [Serializable]
    public class CheckIsDistanceGreater : Leaf
    {
        [SerializeField, Range(0.1f, 50f)] private float distance = 4f;

        private NavMeshAgent agent;
        private Vector3 targetPos;

        public void StartLeaf(NavMeshAgent navMeshAgent)
        {
            agent = navMeshAgent;
        }
        public void UpdateLeaf(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }

        private NodeStates Check()
        {
            float targetDistance = Vector3.Distance(agent.transform.position, targetPos);
            return targetDistance > distance ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }

        public override NodeStates Evaluate()
        {
            switch (Check())
            {
                case NodeStates.FAILURE:
                    nodeState = NodeStates.FAILURE;
                    return nodeState;
                case NodeStates.RUNNING:
                    nodeState = NodeStates.RUNNING;
                    return nodeState;
                case NodeStates.SUCCESS:
                    nodeState = NodeStates.SUCCESS;
                    return nodeState;
                default:
                    nodeState = NodeStates.FAILURE;
                    return nodeState;
            }
        }
    }
}
