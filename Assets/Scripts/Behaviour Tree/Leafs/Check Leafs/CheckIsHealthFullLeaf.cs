using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    [Serializable]
    public class CheckIsHealthFullLeaf : CheckLeaf
    {
        [ShowInInspector] private NodeStates State => NodeState;

        private int curHealth;
        private int maxHealth;

        public void SetFieldsOnUpdate(int currentHealth, int maximumHealth)
        {
            curHealth = currentHealth;
            maxHealth = maximumHealth;
        }

        private NodeStates Check()
        {
            return curHealth == maxHealth ? NodeStates.SUCCESS : NodeStates.FAILURE;
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

