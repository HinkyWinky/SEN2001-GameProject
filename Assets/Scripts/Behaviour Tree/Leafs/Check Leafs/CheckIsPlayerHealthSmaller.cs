using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable] public class CheckIsPlayerHealthSmaller : Leaf
    {
        [SerializeField, Min(1)] private int compareHealth = 3;
        private int playerHealth;

        public void UpdateLeaf(int playerHealthValue)
        {
            playerHealth = playerHealthValue;
        }

        private NodeStates Check()
        {
            return playerHealth < compareHealth ? NodeStates.SUCCESS : NodeStates.FAILURE;
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

