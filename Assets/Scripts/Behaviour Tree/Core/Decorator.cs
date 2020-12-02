namespace BehaviourTree
{
    public abstract class Decorator : Node
    {
        protected Node childNode;

        public abstract override NodeStates Evaluate();
    }
}
