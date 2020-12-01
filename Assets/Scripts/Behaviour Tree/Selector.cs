using System.Collections.Generic;

namespace BehaviourTree
{
    public class Selector : Node
    {
        protected List<Node> nodes = new List<Node>();

        public Selector(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        // return first success one
        public override NodeStates Evaluate()
        {
            foreach (Node node in nodes)
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

