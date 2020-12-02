using System.Collections.Generic;

namespace BehaviourTree
{
    public class Sequence : Composite
    {
        public Sequence(List<Node> childNodesList)
        {
            childNodes = childNodesList;
        }

        // return all of them if all of them are success
        public override NodeStates Evaluate()
        {
            bool anyChildRunning = false;

            foreach (Node node in childNodes)
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

