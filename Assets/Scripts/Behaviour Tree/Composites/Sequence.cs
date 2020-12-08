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

            if (curChildIndex >= childNodes.Count) return NodeStates.SUCCESS; // all children evaluated

            if (childNodeState == NodeStates.SUCCESS) return OnEvaluate(); // move to the next child

            return NodeStates.RUNNING; // keep waiting for the result of this current child
        }
    }
}

