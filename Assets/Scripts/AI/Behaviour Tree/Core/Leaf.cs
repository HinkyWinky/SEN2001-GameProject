using UnityEngine;

namespace Game.AI
{
    public abstract class Leaf : Node
    {
        [HideInInspector] protected BehaviourTreeState btState;
        protected StateMachine Machine => btState.machine;

        public virtual void StartLeaf(BehaviourTreeState behaviourTreeState)
        {
            btState = behaviourTreeState;
        }
        protected override void OnReset() { }
    }
}
