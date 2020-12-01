namespace BehaviourTree
{
    public abstract class ActionLeaf : Node
    {
        public abstract override NodeStates Evaluate();
    }
}
