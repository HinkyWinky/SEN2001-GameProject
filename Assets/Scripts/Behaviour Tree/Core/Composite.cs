using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    public abstract class Composite : Node
    {
        [ShowInInspector, PropertyOrder(-1)] protected NodeStates State => NodeState;

        protected List<Node> childNodes;

        public abstract override NodeStates Evaluate();
    }
}
