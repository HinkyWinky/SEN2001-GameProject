using UnityEngine;

namespace BehaviourTree
{
    public abstract class Node
    {
        protected NodeStates nodeState;
        public NodeStates NodeState => nodeState;

        public abstract NodeStates Evaluate();
    }
}
