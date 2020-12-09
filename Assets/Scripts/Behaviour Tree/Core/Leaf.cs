using UnityEngine;

namespace BehaviourTree
{
    public abstract class Leaf : Node
    {
        [HideInInspector] protected BehaviourTreeState btState;

        public virtual void StartLeaf(BehaviourTreeState behaviourTreeState)
        {
            btState = behaviourTreeState;
        }
        protected override void OnReset() { }
    }
}
