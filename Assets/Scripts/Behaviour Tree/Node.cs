namespace BehaviourTree
{
    public enum NodeStates { FAILURE, RUNNING, SUCCESS };

    [System.Serializable]
    public abstract class Node
    {
        protected NodeStates nodeState;
        public NodeStates NodeState => nodeState;

        public Node() { }

        public abstract NodeStates Evaluate();
    }
}
