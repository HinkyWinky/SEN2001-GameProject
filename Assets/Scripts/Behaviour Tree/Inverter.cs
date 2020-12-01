namespace BehaviourTree
{
    public class Inverter : Node
    {
        private Node node;

        public Inverter(Node node)
        {
            this.node = node;
        }

        public override NodeStates Evaluate()
        {
            switch (node.Evaluate())
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
