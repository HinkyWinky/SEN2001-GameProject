using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Sequence : Node
    {
        protected List<Node> nodes = new List<Node>();

        public Sequence(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        // return all of them if all of them are success
        public override NodeStates Evaluate()
        {
            bool anyChildRunning = false;

            foreach (Node node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        nodeState = NodeStates.FAILURE;
                        return nodeState;
                    case NodeStates.RUNNING:
                        anyChildRunning = true;
                        continue;
                    case NodeStates.SUCCESS:
                        continue;
                    default:
                        nodeState = NodeStates.SUCCESS;
                        return nodeState;
                }
            }
            nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
            return nodeState;
        }
    }
}

