using System.Collections.Generic;

namespace BehaviourTree
{
    public class Sequence : Composite
    {
        public Sequence(List<Node> childNodesList)
        {
            childNodes = childNodesList;
        }

        public override NodeStates OnEvaluate()
        {
            NodeStates childNodeState = childNodes[curChildIndex].Evaluate();

            switch (childNodeState)
            {
                case NodeStates.FAILURE:
                    return childNodeState;
                case NodeStates.SUCCESS:
                    curChildIndex++;
                    break;
            }

            if (curChildIndex >= childNodes.Count)
            {
                return NodeStates.SUCCESS;
            }

            return childNodeState == NodeStates.SUCCESS ? OnEvaluate() : NodeStates.RUNNING;
        }
    }
}

