using System;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    [Serializable]
    public class ActionMove : Leaf
    {
        [SerializeField, Range(0.05f, 50f)] private float stopDistanceThreshold = 1f;

        private NavMeshAgent agent;
        private Vector3 targetPos;
        private Vector3 destination;
        private NavMeshPath path;

        public void StartLeaf(NavMeshAgent navMeshAgent, ref NavMeshPath navMeshPath)
        {
            agent = navMeshAgent;
            path = navMeshPath;
        }
        public void UpdateLeaf(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }

        private NodeStates Action()
        {
            destination = targetPos + (agent.transform.position - targetPos).normalized * stopDistanceThreshold;  
            if (!(Vector3.Distance(agent.transform.position, targetPos) > stopDistanceThreshold)) return NodeStates.SUCCESS;

            agent.CalculatePath(destination, path);
            if (path.status != NavMeshPathStatus.PathComplete) return NodeStates.FAILURE;

            agent.SetDestination(destination);
            return NodeStates.RUNNING;
        }

        public override NodeStates Evaluate()
        {
            switch (Action())
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
