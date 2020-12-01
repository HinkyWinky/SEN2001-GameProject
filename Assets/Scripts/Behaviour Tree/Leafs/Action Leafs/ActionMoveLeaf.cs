using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    [Serializable]
    public class ActionMoveLeaf : ActionLeaf
    {
        [ShowInInspector] private NodeStates State => NodeState;

        [SerializeField] private float stopDistance = 3f;

        private NavMeshAgent agent;
        private Vector3 targetPos;
        private Vector3 destination;
        private NavMeshPath path;

        public void SetFieldsOnStart(NavMeshAgent navMeshAgent, ref NavMeshPath navMeshPath)
        {
            agent = navMeshAgent;
            path = navMeshPath;
        }
        public void SetFieldsOnUpdate(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }

        private NodeStates Action()
        {
            return Move();
        }

        private NodeStates Move()
        {
            Vector3 direction = (agent.transform.position - targetPos).normalized;
            Vector3 targetPosition = targetPos + direction * stopDistance;
            agent.CalculatePath(targetPosition, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                if (Vector3.Distance(destination, targetPos) > stopDistance)
                {
                    destination = targetPosition;
                    agent.SetDestination(destination);
                    return NodeStates.RUNNING;
                }
                return NodeStates.SUCCESS;
            }
            return NodeStates.FAILURE;
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
