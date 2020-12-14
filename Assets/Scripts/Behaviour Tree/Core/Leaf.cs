using UnityEngine;

namespace Game.AI
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
