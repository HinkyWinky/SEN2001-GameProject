using System;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class Selector : Composite
    {
        public Selector(List<Node> childNodes)
        {
            base.childNodes = childNodes;
        }

        // return first success one
        public override NodeStates Evaluate()
        {
            foreach (Node node in childNodes)
            {
                switch (node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        continue;
                    case NodeStates.RUNNING:
                        nodeState = NodeStates.RUNNING;
                        return nodeState;
                    case NodeStates.SUCCESS:
                        nodeState = NodeStates.SUCCESS;
                        return nodeState;
                    default:
                        continue;
                }
            }
            nodeState = NodeStates.FAILURE;
            return nodeState;
        }
    }
}
