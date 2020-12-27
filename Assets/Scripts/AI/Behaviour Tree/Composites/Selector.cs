using System.Collections.Generic;

namespace Game.AI
{
    public class Selector : Composite
    {
        public Selector(List<Node> childNodesList)
        {
            childNodes = childNodesList;
        }

        public override NodeStates OnEvaluate()
        {
            if (curChildIndex >= childNodes.Count)
            {
                return NodeStates.FAILURE;
            }

            NodeStates childNodeState = childNodes[curChildIndex].Evaluate();

            switch (childNodeState)
            {
                case NodeStates.FAILURE:
                    curChildIndex++;
                    break;
                case NodeStates.SUCCESS:
                    return NodeStates.SUCCESS;
            }

            return NodeStates.RUNNING;
        }
    }
}
