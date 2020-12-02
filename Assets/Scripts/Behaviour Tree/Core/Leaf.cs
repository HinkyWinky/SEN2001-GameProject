using Sirenix.OdinInspector;

namespace BehaviourTree
{
    public abstract class Leaf : Node
    {
        [ShowInInspector, PropertyOrder(-1)] protected NodeStates State => NodeState;

        public abstract override NodeStates Evaluate();
    }
}
