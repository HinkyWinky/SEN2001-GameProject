namespace BehaviourTree
{
    public class Inverter : Decorator
    {
        public Inverter(Node node)
        {
            childNode = node;
        }

        public override NodeStates Evaluate()
        {
            switch (childNode.Evaluate())
            {
                case NodeStates.FAILURE:
                    nodeState = NodeStates.SUCCESS;
                    return nodeState;
                case NodeStates.RUNNING:
                    nodeState = NodeStates.RUNNING;
                    return nodeState;
                case NodeStates.SUCCESS:
                    nodeState = NodeStates.FAILURE;
                    return nodeState;
            }
            nodeState = NodeStates.SUCCESS;
            return nodeState;
        }
    }
}
