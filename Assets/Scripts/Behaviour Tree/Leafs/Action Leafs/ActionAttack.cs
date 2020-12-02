using System;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    [Serializable]
    public class ActionAttack : Leaf
    {
        [SerializeField, Range(0.05f, 50f)] private float attackDistanceThreshold = 1f;
        [SerializeField] private string animationMotionName = default;
        [SerializeField, Min(0)] private float animationDuration = 1f;
        [SerializeField, Range(0f, 1f)] private float animationFadeDuration = 0f;

        private NavMeshAgent agent;
        private NavMeshPath path;
        private Animator anim;
        private AnimatorX animX;

        private Vector3 targetPos;

        private Vector3 destination;
        private bool isAnimRunning;

        public void StartLeaf(NavMeshAgent navMeshAgent, ref NavMeshPath navMeshPath, Animator animator, AnimatorX animatorX)
        {
            agent = navMeshAgent;
            path = navMeshPath;
            anim = animator;
            animX = animatorX;
        }
        public void UpdateLeaf(Vector3 targetPosition)
        {
            targetPos = targetPosition;
        }

        private NodeStates Action()
        {
            if (Vector3.Distance(agent.transform.position, targetPos) < attackDistanceThreshold)
            {
                if (!isAnimRunning)
                {
                    isAnimRunning = true;
                    animX.StartAnimation(animationMotionName, animationDuration, false, animationFadeDuration);
                    return NodeStates.RUNNING;
                }

                if (animX.IsPlaying(animationMotionName)) return NodeStates.RUNNING;

                isAnimRunning = false;
                return NodeStates.SUCCESS;
            }

            isAnimRunning = false;
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
