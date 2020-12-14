using System;
using Sirenix.OdinInspector;

namespace Game.AI
{
    [Serializable]
    public abstract class Node
    {
        protected NodeStates nodeState = NodeStates.EMPTY;
        [ShowInInspector, ReadOnly, PropertyOrder(-1)] public NodeStates NodeState => nodeState;

        protected int loopCount = 0;
        public bool IsFirstLoop => loopCount == 0;

        public virtual NodeStates Evaluate()
        {
            nodeState = OnEvaluate();

            loopCount++;

            if (nodeState != NodeStates.RUNNING)
            {
                Reset();
            }

            return nodeState;
        }
        public abstract NodeStates OnEvaluate();

        public void Reset()
        {
            loopCount = 0;
            OnReset();
        }
        protected abstract void OnReset();
    }
}
